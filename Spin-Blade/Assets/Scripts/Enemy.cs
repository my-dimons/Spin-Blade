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
    public float health = 1f;
    public float rotateSpeed = 0;

    [Header("Knockback")]
    public float rotationForce = 5f;
    private Rigidbody2D rb;
    private bool isStunned = false;

    [Header("Extra")]
    public GameObject deathParticles;
    public GameObject hitParticles;
    public Color hitColor;
    public Color hitCircleColor;

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

    EnemyManager enemyManager;
    PlayerHealth playerHealth;

    [Header("Special")]
    public bool randomSize;
    public float minSize = 0.5f;
    public float maxSize = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();

        health *= enemyManager.difficulty;
        speed *= enemyManager.difficulty;
        damage *= enemyManager.difficulty;

        if (randomSize)
        {
            float randomScale = Random.Range(minSize, maxSize);
            transform.localScale = new Vector3(randomScale, randomScale, 1f);
        }
    }

    private void FixedUpdate()
    {
        if (!isStunned && target != null)
            EnemyMovement();
    }
    private void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
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
        if (other.CompareTag("Circle"))
        {
            HitCircle();
        }
        else if (other.CompareTag("Player"))
        {
            health -= playerHealth.damage;
            if (health <= 0 && !isStunned)
            {
                Debug.Log("Destroyed by player");
                Death();
            }
            else
            {
                Knockback(target.transform, playerHealth.knockbackForce, playerHealth.stunDuration);
            }
        }
        else if (other.CompareTag("PlayerProjectile"))
        {
            Projectile proj = other.GetComponent<Projectile>();

            health -= proj.damage;

            if (health <= 0 && !isStunned)
            {
                Debug.Log("Destroyed by player");
                GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().AddMoney(value);
                Death();
            }
            else
            {
                if (proj.destroyOnHit)
                    Destroy(other.gameObject);

                Knockback(target.transform, proj.knockbackForce, proj.stunDuration);
            }
        }
    }

    public void Knockback(Transform attacker, float force, float stunDuration)
    {
        Utils.PlayClip(hitSound);
        Utils.SpawnBurstParticle(hitParticles, transform.position, hitColor);

        if (isStunned) return;

        StartCoroutine(StunCoroutine(stunDuration));

        Vector2 direction = (transform.position - attacker.position).normalized;

        rb.linearVelocity = Vector2.zero;
        
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        float spinDirection = Random.value < 0.5f ? -1 : 1;
        rb.AddTorque(rotationForce * spinDirection, ForceMode2D.Impulse);
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

    public void Death()
    {
        playerHealth.Heal(healthGain);

        Utils.PlayClip(deathSound);
        Utils.SpawnBurstParticle(deathParticles, transform.position, hitColor);
        Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(.08f);

        // text
        Color color;
        if (value > 0)
            color = goodMoneyColor;
        else
            color = badMoneyColor;
        Utils.SpawnFloatingText(deathMoneyText, transform.position, "$" + value.ToString("F1"), 6f, 0.3f, 40f, 0.45f, 0.15f, color);

        GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().AddMoney(value);
        if (playerHealth.regenOnKill)
            playerHealth.Heal(playerHealth.killRegenAmount);

        enemyManager.IncreaseDifficulty();
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().kills++;
        Destroy(gameObject);
    }

    public void HitCircle()
    {
        Utils.SpawnBurstParticle(deathParticles, transform.position, hitCircleColor);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().TakeDamage(damage);
        Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(.5f);

        if (moneyGain > 0)
        {
            GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().AddMoney(moneyGain);
            Utils.SpawnFloatingText(deathMoneyText, transform.position, "$" + moneyGain.ToString("F1"), 6f, 0.3f, 40f, 0.45f, 0.15f, goodMoneyColor);
        }
        Destroy(gameObject);
    }
}