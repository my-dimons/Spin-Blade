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

    [Header("Knockback")]
    private Rigidbody2D rb;
    private bool isStunned = false;
    public float rotationForce = 5f;

    EnemyManager enemyManager;
    PlayerHealth playerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();

        health *= enemyManager.difficulty;
        speed *= enemyManager.difficulty;
        damage *= enemyManager.difficulty;
    }

    private void FixedUpdate()
    {
        if (!isStunned)
            EnemyMovement();
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
        RotateTowardsTarget(target);
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime * enemyManager.enemySpeedMultiplier);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Circle"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
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
        GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().AddMoney(value);
        if (playerHealth.regenOnKill)
            playerHealth.Heal(playerHealth.killRegenAmount);

        enemyManager.IncreaseDifficulty();
        Destroy(gameObject);
    }
}