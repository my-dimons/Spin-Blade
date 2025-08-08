using UnityEngine;

public class TriangleProjectile : MonoBehaviour
{
    Projectile projectile;
    public GameObject homingTarget;
    public bool homing;
    public float homingRotationOffset;
    private Vector2 moveDirection;
    public float lifeTime = -1f;

    private void Start()
    {
        projectile = GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError("Projectile component is missing on TriangleProjectile.");
        }
        if (lifeTime > 0f)
        {
            Destroy(gameObject, lifeTime);
        }
    }
    public void Initialize(Vector2 direction, bool home, bool piercing)
    {
        homing = home;
        GetComponent<Projectile>().destroyOnHit = !piercing;

        moveDirection = direction.normalized;

        // Rotate projectile to face that direction
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // -90 if sprite points up
    }

    private void Update()
    {
        if (homing && homingTarget != null)
        {
            moveDirection = homingTarget.transform.position - transform.position;
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            angle += homingRotationOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        transform.position += projectile.speed * Time.deltaTime * (Vector3)moveDirection;
    }
}
