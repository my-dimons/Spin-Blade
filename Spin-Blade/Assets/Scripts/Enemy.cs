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
    public float health = 1f; // how much hits this enemy can take  

    [Header("Knockback")]
    private Rigidbody2D rb;
    private bool isStunned = false;
    public float rotationForce = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

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
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Circle"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            health -= other.GetComponent<PlayerHealth>().damage;
            if (health <= 0)
            {
                Debug.Log("Destroyed by player");
                GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().AddMoney(value);
                Destroy(gameObject);
            }
            else
            {
                Knockback(target.transform);
            }
        }
    }

    public void Knockback(Transform attacker)
    {
        if (isStunned) return;

        StartCoroutine(StunCoroutine());

        Vector2 direction = (transform.position - attacker.position).normalized;

        rb.linearVelocity = Vector2.zero;
        
        rb.AddForce(direction * GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().knockbackForce, ForceMode2D.Impulse);

        float spinDirection = Random.value < 0.5f ? -1 : 1;
        rb.AddTorque(rotationForce * spinDirection, ForceMode2D.Impulse);
    }
    private IEnumerator StunCoroutine()
    {
        isStunned = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;

        yield return new WaitForSeconds(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().stunDuration);

        isStunned = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
    }
}