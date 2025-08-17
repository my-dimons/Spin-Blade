using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthAndDamage : MonoBehaviour
{
    [Header("-- Health --")]
    public float maxHeath = 1;
    public float currentHealth;
    [Space(10)]
    public int revives;
    public float regenPerSecond;
    public float killRegenAmount = 0f; // amount of health to regen on kill

    [Space(10)]
    [Header("Health Display")]
    public Image healthBar;
    public GameObject deathScreen;
    [Space(10)]
    [Header("Damage Flashes")]
    public DamageFlash circleDamageFlash;
    public Color circleDamageFlashColor = Utils.ColorFromHex("#FF4E4E");
    public Color circleMoneyGainHitFlashColor = Utils.ColorFromHex("#7CFF85");
    public Color circleFullHealFlashColor = Utils.ColorFromHex("#FFE45B");

    bool dead;
    private float oldHealth;
    private float oldMaxHealth;

    [Header("-- Health --")]

    [Space(20)]

    [Header("-- Damage --")]
    public float damage = 1;
    public float sizeMultiplier = 1f;
    private Vector2 baseSize;
    [Space(3)]
    [Header("Knockback")]
    public AnimationCurve knockbackCurve;
    public float knockbackDistance = 3f;
    public float knockbackDuration = 0.2f;

    [Header("-- Damage --")]

    [Space(20)]

    [Header("-- Mini Saws --")]
    public List<GameObject> miniSaws;
    public GameObject miniSawPrefab;
    public GameObject miniSawWaypointsParent;
    GameObject[] miniSawWaypoints;
    [Header("Stats")]
    public float miniSawBaseSpeed = 1f;
    public float hexagonDamage = 1f;

    [Space(10)]

    [Header("-- Ranged Triangles --")]
    public bool unlockedRangedTriangles; // triangles that shoot at enemies
    public GameObject rotationPivot; // used to properly rotate the triangles
    public GameObject trianglePrefab;
    [Header("Stats")]
    public float triangleDamage = 1f;
    public float triangleSpeed = 1f;
    public float triangleFireRate = 0.8f; // seconds between shots
    [Header("Unlocks")]
    public bool autofireTriangles;
    public bool homingTriangles;
    public bool piercingTriangles;
    float triangleFireTimer = 0f;

    [Space(10)]

    [Header("-- Exploding Circles --")]
    public bool explodingCircle;

    [Header("Visuals")]
    public GameObject explodingCirclePrefab;
    public Image explodingCircleVisualCooldown;
    public float explodingCircleVisualFinalSize;
    public AnimationCurve explodingCircleSizeAnimationCurve;
    public AnimationCurve explodingCircleOpacityAnimationCurve;

    public float explodingCircleAnimationDuration;

    [Header("Stats")]
    public float explodingCircleCooldown = 10f;
    [Tooltip("playerDamage * thisvar")]
    public float explodingCircleDamageMultiplier = 1f;
    [Header("Unlocks")]
    public bool explodingCircleKnockback;

    float explodingCircleCooldownTimer = 0f;

    [Header("-- Mines --")]
    public bool mines;
    public GameObject minePrefab;
    [Header("Stats")]
    public float minesCooldown = 6f;
    public float minesLifetime = 8f;
    public float mineDamageMultiplier = 1f;
    public float mineExplosionRadius = 6f;
    public int mineHitsBeforeDeath = 1;

    float minesCooldownTimer = 0f;
    float minesRadiusDivisor = 1.3f; // make radius smaller so most mines are in the camera view
    [Header("Unlocks")]
    public bool explodingMines;

    [Space(10)]
    [Header("Audio")]
    public AudioClip deathSound;
    public AudioClip hitSound;
    public AudioClip fullHealthSound;
    public AudioClip explodingCircleSound;

    private void OnValidate()
    {
        currentHealth = maxHeath;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get all children of minisawwaypoint parent
        miniSawWaypoints = miniSawWaypointsParent.GetComponentsInChildren<Transform>()
                                                .Where(t => t != miniSawWaypointsParent.transform) // exclude parent
                                                .Select(t => t.gameObject)
                                                .ToArray();

        currentHealth = maxHeath;
        baseSize = transform.localScale;

        if (GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
        {
            regenPerSecond = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = baseSize * sizeMultiplier;

        // triangle timer
        triangleFireTimer += Time.deltaTime;
        if ((Input.GetKeyDown(KeyCode.Space) || autofireTriangles) && triangleFireTimer >= triangleFireRate && unlockedRangedTriangles)
        {
            triangleFireTimer = 0f;
            SpawnTriangle();
        }

        // exploding circle timer
        if (explodingCircle)
            explodingCircleCooldownTimer += Time.deltaTime;
        if ((explodingCircle && explodingCircleCooldownTimer >= explodingCircleCooldown))
        {
            explodingCircleCooldownTimer = 0f;
            ExplodeCircle(Vector2.zero, damage * explodingCircleDamageMultiplier, explodingCircleVisualFinalSize, explodingCircleKnockback);
        }
        // exploding circle visual cooldown
        if (explodingCircle)
        {
            explodingCircleVisualCooldown.fillAmount = (float)explodingCircleCooldownTimer / explodingCircleCooldown;
        } else
        {
            // full fill
            explodingCircleVisualCooldown.fillAmount = 1;
        }

        // mines
        if (mines)
        {
            minesCooldownTimer += Time.deltaTime;
        }
        if (mines && minesCooldownTimer >= minesCooldown || Input.GetKeyDown(KeyCode.R))
        {
            minesCooldownTimer = 0f;
            SpawnMine();
        }

        // clamp health & add regen
        float regenAmount = currentHealth * regenPerSecond / 100;
        currentHealth = Mathf.Clamp(currentHealth += regenAmount * Time.deltaTime, 0, maxHeath);

        // full health ping
        if (currentHealth >= maxHeath && oldHealth < maxHeath && oldHealth != oldMaxHealth)
        {
            Utils.PlayAudioClip(fullHealthSound, 0.7f);
            circleDamageFlash.Flash(circleFullHealFlashColor);
        }

        // update vars for health ping
        oldHealth = currentHealth;
        oldMaxHealth = maxHeath;

        // health bar visual fill
        healthBar.fillAmount = (float)currentHealth / maxHeath;

        // death if health is 0
        if (currentHealth <= 0 && !dead)
        {
            Death();
        }
    }

    [ContextMenu("Spawn Mini Sawblade")]
    public void SpawnSaw()
    {
        GameObject randomSaw = miniSawWaypoints[Random.Range(0, miniSawWaypoints.Length)];
        GameObject saw = Instantiate(miniSawPrefab, randomSaw.transform.position, Quaternion.identity);
        miniSaws.Add(saw);
        saw.GetComponent<PlayerMiniSaw>().waypoints = new List<GameObject>(miniSawWaypoints);
        saw.GetComponent<PlayerMiniSaw>().currentWaypoint = randomSaw;
    }

    public void SpawnTriangle()
    {
        GameObject triangle = Instantiate(trianglePrefab, transform.position, Quaternion.identity);
        triangle.GetComponent<Projectile>().damage = triangleDamage;
        triangle.GetComponent<Projectile>().speed = triangleSpeed;

        // Calculate direction AWAY from center
        Vector2 direction = (transform.position - rotationPivot.transform.position).normalized;

        triangle.GetComponent<TriangleProjectile>().Initialize(direction, homingTriangles, piercingTriangles);

    }

    public void ExplodeCircle(Vector2 spawnPos, float circleDamage, float finalSize, bool knockback)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // damage all enemies, and do knockback if unlocked

        foreach (GameObject enemy in enemies)
        {
            // must be inside the final circle size to do damage
            if (Vector2.Distance(spawnPos, enemy.transform.position) <= finalSize)
            {
                if (knockback)
                    enemy.GetComponent<Enemy>().TakeDamage(enemy.transform, circleDamage, knockbackDistance, knockbackDuration, knockbackCurve, true);
                else
                    enemy.GetComponent<Enemy>().TakeDamage(enemy.transform, circleDamage, 1, 0.8f, knockbackCurve, true); // do a little knockback to 'stun' the enemy
            }
        }

        StartCoroutine(ExplodingCircleVisual());

        float screenshakeDuration = .6f;
        Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(screenshakeDuration);
        Utils.PlayAudioClip(explodingCircleSound, 0.8f);


        IEnumerator ExplodingCircleVisual()
        {
            GameObject circle = Instantiate(explodingCirclePrefab, spawnPos, Quaternion.identity);
            Transform circleTransform = circle.transform;
            SpriteRenderer sr = circle.GetComponent<SpriteRenderer>();

            Vector3 startScale = circleTransform.localScale;
            Vector3 endScale = finalSize * Vector3.one * 2; // multiply by 2 because for some reason the scale is half the size of the sprite or smth ¯\(°_o)/¯ idk it just works

            float startAlpha = sr != null ? sr.color.a : 1f;
            float endAlpha = 0f;

            float time = 0f;

            while (time < explodingCircleAnimationDuration)
            {
                float t = time / explodingCircleAnimationDuration;

                // Scale
                float scaleValue = explodingCircleSizeAnimationCurve.Evaluate(t);
                circleTransform.localScale = Vector3.LerpUnclamped(startScale, endScale, scaleValue);

                // Opacity
                if (sr != null)
                {
                    float alphaValue = explodingCircleOpacityAnimationCurve.Evaluate(t);
                    Color c = sr.color;
                    c.a = Mathf.LerpUnclamped(startAlpha, endAlpha, alphaValue);
                    sr.color = c;
                }

                time += Time.deltaTime;
                yield return null;
            }

            // Snap final state
            circleTransform.localScale = endScale;
            if (sr != null)
            {
                Color c = sr.color;
                c.a = endAlpha;
                sr.color = c;
            }

            Destroy(circle);
        }
    }

    void SpawnMine()
    {
        // spawn a mine within the spawning enemy circle
        Vector2 spawnPos = GetRandomPointInDonut(4.5f, GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>().radius / minesRadiusDivisor);

        GameObject mine = Instantiate(minePrefab, spawnPos, Quaternion.identity);

        // set mine stats
        float knockbackDivisor = 2;
        PlayerMine mineScript = mine.GetComponent<PlayerMine>();

        // msc
        mineScript.damage = damage * mineDamageMultiplier;
        mineScript.curve = knockbackCurve;
        mineScript.lifetime = minesLifetime;
        mineScript.hitsBeforeDeath = mineHitsBeforeDeath;

        // kb
        mineScript.knockback = knockbackDistance / knockbackDivisor;
        mineScript.stunDuration = knockbackDuration / knockbackDivisor;

        // unlocks
        mineScript.explode = explodingMines;
        mineScript.explosionRadius = mineExplosionRadius;

       Vector3 GetRandomPointInDonut(float innerRadius, float outerRadius)
       {
           // Pick a random angle
           float angle = Random.Range(0f, Mathf.PI * 2f);

           // Pick a random distance between inner and outer
           float radius = Random.Range(innerRadius, outerRadius);

           // Convert polar → Cartesian
           float x = Mathf.Cos(angle) * radius;
           float y = Mathf.Sin(angle) * radius;

           return new Vector3(x, y, 0f);
       }
    }

    private void OnDrawGizmos()
    {
        // mine spawning circles (make sure these are correct)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Vector2.zero, GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>().radius / minesRadiusDivisor);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Vector2.zero, 4.5f);
    }

    void Death()
    {
        // revive and kill all enemies
        if (revives > 0)
        {
            currentHealth = maxHeath;
            revives--;

            KillAllEnemies();

            return;
        }

        dead = true;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().tutorialText.gameObject.SetActive(false); 
        Utils.PlayAudioClip(deathSound, 1.3f);
        Time.timeScale = 0;
        deathScreen.SetActive(true);
    }

    public void TakeDamage(float damage, bool flashMoney = false)
    {
        Utils.PlayAudioClip(hitSound, 1f);
        currentHealth -= damage;

        if (currentHealth > 0 && !flashMoney)
            circleDamageFlash.Flash(circleDamageFlashColor);
        else if (currentHealth > 0 && flashMoney)
            circleDamageFlash.Flash(circleMoneyGainHitFlashColor);

        Mathf.Clamp(currentHealth, 0, maxHeath);
    }
    public void Heal(float heal)
    {
        currentHealth += heal;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHeath);
    }
    public void IncreaseMaxHealth(float amount)
    {
        maxHeath += amount;
        currentHealth += amount;
        Mathf.Clamp(currentHealth, 0, maxHeath);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemyScript = collision.GetComponent<Enemy>();
            enemyScript.maxHealth -= damage;

            enemyScript.TakeDamage(transform, damage, knockbackDistance, knockbackDuration, knockbackCurve, true);
        }
    }

    public void KillAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Enemy>().Death();
        }
    }
}
