using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public GameObject target;
    public float minTargetDistance = 0.05f;
    public float speed = 5f;
    public float rotateMultiplier = 1f;

    [Header("Other Stats")]
    public float value; // how much this enemy is worth when killed
    public float damage = 1f;
    public float maxHealth = 1f;
    public float currentHealth;
    public float rotateSpeed = 0;

    [Header("Knockback")]
    private Rigidbody2D rb;
    private bool isStunned = false;

    [Header("Extra")]
    public Color damageFlashColor = Color.white;
    public GameObject deathParticles;
    public GameObject hitParticles;
    public Color hitColor;

    public GameObject deathMoneyText;
    public AudioClip deathSound;
    public AudioClip hitSound;

    public Color goodMoneyColor;
    public Color badMoneyColor;

    // when hitting circle
    public float moneyGain;
    // when killed by player (mainly for dealing damage when player hits enemy)
    public float healthGain;

    public float spawnRate = 1; // 1 is ALWAYS SPAWN (when selected), value is 0 - 1

    public bool isBoss;

    EnemyManager enemyManager;
    PlayerHealthAndDamage playerHealth;

    [Header("Special")]
    public bool damageFromProjectiles = true;
    public bool triggerEventOnDeath;
    public bool randomSize;
    public float minSize = 0.5f;
    public float maxSize = 1.5f;

    private void OnValidate()
    {
        currentHealth = maxHealth;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();
        rb = GetComponent<Rigidbody2D>();

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
            float randomScaleX = Random.Range(minSize, maxSize);
            float randomScaleY = Random.Range(minSize, maxSize);
            transform.localScale = new Vector3(randomScaleX, randomScaleY, 1f);
        }

        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        if (!isStunned && target != null)
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

            currentHealth -= proj.damage;

            if (other.GetComponent<TriangleProjectile>())
            {
                other.GetComponent<TriangleProjectile>().homingTarget = null;
            }

            if (currentHealth <= 0)
            {
                Debug.Log("Destroyed by player");
                GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().AddMoney(value);
                Death();
            }
            else
            {
                if (proj.destroyOnHit)
                    Destroy(other.gameObject);

                TakeDamage(other.transform, proj.knockbackForce, proj.stunDuration, false);
            }
        }

        if (other.CompareTag("Circle") && currentHealth > 0)
        {
            HitCircle();
        }
    }

    public void TakeDamage(Transform attacker, float force, float stunDuration, bool knockback = true, bool stun = true)
    {
        Utils.PlayClip(hitSound);
        Vector3 particlePos = (attacker.position + transform.position) / 2f;
        Utils.SpawnBurstParticle(hitParticles, particlePos, hitColor);
        GetComponent<DamageFlash>().Flash(damageFlashColor);


        if (stun)
            StartCoroutine(StunCoroutine(stunDuration));

        // knockback
        if (knockback)
        {
            Knockback(force);
        }

    }

    private void Knockback(float force)
    {
        Vector2 direction = (transform.position - target.transform.position).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private IEnumerator StunCoroutine(float stunDuration)
    {
        isStunned = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
    }

    public void Death(bool playerStatGain = true)
    {
        playerHealth.Heal(healthGain);

        Utils.PlayClip(deathSound, 0.8f);
        Utils.SpawnBurstParticle(deathParticles, transform.position, hitColor);
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
            MoneyManager moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();

            if (!GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
                Utils.SpawnFloatingText(deathMoneyText, transform.position, "$" + GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().CalculateMoney(value).ToString("F1"), 6f, 0.3f, 40f, 0.45f, 0.15f, color);

            moneyManager.AddMoney(value);

            playerHealth.Heal(playerHealth.killRegenAmount);

            if (triggerEventOnDeath && !enemyManager.eventHappening)
                enemyManager.StartRandomEvent();

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().kills++;
        }


        enemyManager.IncreaseDifficulty();
        Destroy(gameObject);
    }

    public void HitCircle()
    {
        Utils.SpawnBurstParticle(deathParticles, transform.position, hitColor);
        Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(.5f);

        if (moneyGain > 0)
        {
            GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().AddMoney(moneyGain);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>().TakeDamage(damage, true);

            // money text popup
            Utils.SpawnFloatingText(deathMoneyText, transform.position, "$" + GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().CalculateMoney(moneyGain).ToString("F1"), 6f, 0.3f, 40f, 0.45f, 0.15f, goodMoneyColor);
        } else
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>().TakeDamage(damage);
        Destroy(gameObject);
    }
}