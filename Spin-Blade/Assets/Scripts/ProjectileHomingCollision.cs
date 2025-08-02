using UnityEngine;

public class ProjectileHomingCollision : MonoBehaviour
{
    public TriangleProjectile projectile;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && projectile.homingTarget == null && collision.GetComponent<Enemy>().damageFromProjectiles)
        {
            projectile.homingTarget = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == projectile.homingTarget)
            projectile.homingTarget = null;
    }
}
