using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("-- Movement --")]
    public GameObject target;
    public float speed = 5f;
    public float rotateMultiplier = 1f;

    [Header("-- Stats --")]
    public float value; // how much this enemy is worth when killed
    public MoneyManager.Currency valueCurrencyType = MoneyManager.Currency.money;
    [Space(8)]
    public float damage = 1f;
    [Space(8)]
    public float maxHealth = 1f;
    public float currentHealth;
    [Space(8)]
    public float rotateSpeed = 0;

    private Coroutine knockbackRoutine;

    [Header("-- Extra --")]
    [Header("On Hit")]
    public Color hitCircleColor = Utils.ColorFromHex("#FF4E4E");
    Color damageFlashColor = Color.white;

    public GameObject deathParticles;
    public GameObject hitParticles;

    [Header("Hitting circle")]
    public Color goodMoneyColor;
    public Color badMoneyColor;

    [Header("Audio")]
    public GameObject deathMoneyText;
    public AudioClip deathSound;
    public AudioClip hitSound;


    // when hitting circle
    public float circleHitMoneyGain;
    // when killed by player (mainly for dealing damage when player hits enemy)
    public float healthGain;

    public float spawnRate = 1; // 1 is ALWAYS SPAWN (when selected), value is 0 - 1

    public bool isBoss;


    [Header("Special")] // todo: move this to a new script
    public bool damageFromProjectiles = true;
    public bool triggerEventOnDeath;
    public bool randomSize;
    public float minSize = 0.5f;
    public float maxSize = 1.5f;

    // death
    public event Action OnDeath;
    private bool isDead = false;

    private void OnValidate()
    {
        currentHealth = maxHealth;
    }


    private MoneyManager moneyManager;
    EnemyManager enemyManager;
    PlayerHealthAndDamage playerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();
        moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();

        speed *= enemyManager.difficulty;

        if (!isBoss)
        {
            damage *= enemyManager.difficulty;
            maxHealth *= enemyManager.difficulty;
        }
        else
        {
            maxHealth *= enemyManager.difficulty * Mathf.Clamp(playerHealth.damage, 1, Mathf.Infinity) * enemyManager.bossHealthMultiplier;
            damage = Mathf.Clamp(playerHealth.maxHeath / damage, 1, Mathf.Infinity);
        }

        if (randomSize)
        {
            float randomScaleX = UnityEngine.Random.Range(minSize, maxSize);
            float randomScaleY = UnityEngine.Random.Range(minSize, maxSize);
            transform.localScale = new Vector3(randomScaleX, randomScaleY, 1f);
        }

        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        if (target != null)
            EnemyMovement();
    }
    private void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

        if (enemyManager.eventHappening && triggerEventOnDeath)
        {
            Death(false);
        }

        // clamp health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void RotateTowardsTarget(GameObject target)
    {
        Vector3 vectorToTarget = target.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotateMultiplier;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
    }

    void EnemyMovement()
    {
        if (rotateSpeed == 0)
            RotateTowardsTarget(target);

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime * enemyManager.enemySpeedMultiplier);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile") && damageFromProjectiles)
        {
            Projectile proj = other.GetComponent<Projectile>();

            if (other.GetComponent<TriangleProjectile>())
            {
                other.GetComponent<TriangleProjectile>().homingTarget = null;
            }
            else
            {
                if (proj.destroyOnHit)
                    Destroy(other.gameObject);

                TakeDamage(other.transform, proj.damage, proj.knockbackForce, proj.stunDuration, playerHealth.knockbackCurve);
            }
        }

        if (other.CompareTag("Circle") && currentHealth > 0)
        {
            HitCircle();
        }
    }

    public void TakeDamage(Transform attacker, float damage, float distance = 0, float duration = 0, AnimationCurve curve = null, bool knockback = false)
    {
        Debug.Log("ENEMY COLLISIONS");
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Death();
            return;
        }

        Utils.PlayAudioClip(hitSound);
        Vector3 particlePos = (attacker.position + transform.position) / 2f;
        Utils.SpawnBurstParticle(hitParticles, particlePos, hitCircleColor);
        GetComponent<DamageFlash>().Flash(damageFlashColor);


        // knockback
        if (knockback)
        {
            //Knockback(force);
            KnockbackFrom(Vector2.zero, distance, duration, curve);
        }

    }
    /// <summary>
    /// Moves the enemy away from a point by a given distance, following an animation curve.
    /// </summary>
    public void KnockbackFrom(Vector3 centerPoint, float distance, float duration, AnimationCurve curve)
    {
        // Cancel any ongoing knockback
        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackRoutine(centerPoint, distance, duration, curve));
    }

    private IEnumerator KnockbackRoutine(Vector3 centerPoint, float distance, float knockbackDuration, AnimationCurve knockbackCurve)
    {
        Vector3 startPos = transform.position;

        // Direction away from the point
        Vector3 dir = (startPos - centerPoint).normalized;

        // Calculate knockback target once
        Vector3 endPos = startPos + dir * distance;

        float time = 0f;
        while (time < knockbackDuration)
        {
            float t = time / knockbackDuration;
            float curveValue = knockbackCurve.Evaluate(t); // Curve mapping 0 → 1

            // Smoothly move along the curve
            transform.position = Vector3.Lerp(startPos, endPos, curveValue);
            // If using physics:
            // rb.MovePosition(Vector3.Lerp(startPos, endPos, curveValue));

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // Snap to end
        knockbackRoutine = null;
    }
    public void Death(bool playerStatGain = true)
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("enemy death");

        playerHealth.Heal(healthGain);

        Utils.PlayAudioClip(deathSound, 0.8f);
        Utils.SpawnBurstParticle(deathParticles, transform.position, hitCircleColor);
        Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(.08f);

        // text
        Color color;
        if (value > 0)
            color = goodMoneyColor;
        else
        {
            color = badMoneyColor;
        }

        if (playerStatGain)
        {
            if (!GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
                Utils.SpawnFloatingText(deathMoneyText, transform.position, moneyManager.CalculateMoneyString(moneyManager.CalculateCurrency(value, valueCurrencyType), 1, valueCurrencyType), 6f, 0.3f, 40f, 0.45f, 0.15f, color);
            
            moneyManager.AddCurrency(value, valueCurrencyType);

            playerHealth.Heal(playerHealth.killRegenAmount);

            if (triggerEventOnDeath && !enemyManager.eventHappening)
                enemyManager.StartRandomEvent();

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().kills++;
        }

        // trigger death event
        OnDeath?.Invoke();

        enemyManager.IncreaseDifficulty();
        Destroy(gameObject);
    }

    public void HitCircle()
    {
        Utils.SpawnBurstParticle(deathParticles, transform.position, hitCircleColor);
        Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(.5f);

        if (circleHitMoneyGain > 0)
        {
            moneyManager.AddCurrency(circleHitMoneyGain, valueCurrencyType);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>().TakeDamage(damage, true);

            // money text popup
            Utils.SpawnFloatingText(deathMoneyText, transform.position, moneyManager.CalculateMoneyString(moneyManager.CalculateCurrency(circleHitMoneyGain), 1, valueCurrencyType), 6f, 0.3f, 40f, 0.45f, 0.15f, goodMoneyColor);
        } else
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>().TakeDamage(damage);

        Destroy(gameObject);
    }
}