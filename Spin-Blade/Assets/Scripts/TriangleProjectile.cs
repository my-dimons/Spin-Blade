using UnityEngine;

public class TriangleProjectile : MonoBehaviour
{
    Projectile projectile;
    public bool homing;
    private Vector2 moveDirection;

    private void Start()
    {
        projectile = GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError("Projectile component is missing on TriangleProjectile.");
        }
    }
    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;

        // Rotate projectile to face that direction
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // -90 if sprite points up
    }

    private void Update()
    {
        if (homing)
        {
            // do homing things
        }
        transform.position += (Vector3)moveDirection * projectile.speed * Time.deltaTime;
    }
}
