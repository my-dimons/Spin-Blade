using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class RangerEnemy : MonoBehaviour
{
    public GameObject enemyProjectile;

    [Header("Stats")]
    public float enemyProjectileSpeed = 4;
    public float spawnDelay = 6f;
    public float projectileDamage = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnProjectile), spawnDelay, spawnDelay);
    }

    void SpawnProjectile()
    {
        Enemy projectile = Instantiate(enemyProjectile, transform.position, Quaternion.identity).GetComponent<Enemy>();

        projectile.gameObject.transform.parent = GetComponent<Enemy>().target.transform;

        projectile.target = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>().enemyParent;
        projectile.speed = enemyProjectileSpeed;
        projectile.damage = projectileDamage;

        Destroy(projectile, 60f);
    }
}
