using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public GameObject target; // target is also parent
    public float minTargetDistance = 0.05f;
    public float speed = 5f;
    public float rotateMultiplier = 1f;

    [Header("Other Stats")]
    public float value; // how much this enemy is worth when killed


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
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
        Debug.Log("OVERLLAPING ENEMY");
        if (other.CompareTag("EnemyParent"))
        {
            Debug.Log("Enemy reached the target!");
            Destroy(gameObject);
        }
    }
}
