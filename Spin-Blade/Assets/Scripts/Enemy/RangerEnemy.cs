using UnityEngine;

public class RangerEnemy : MonoBehaviour
{
    public GameObject enemyProjectile;
    public float enemyProjectileSpeed = 4;
    public float spawnDelay = 6f;
    public float projectileDamage = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), spawnDelay, spawnDelay);
    }

    void SpawnEnemy()
    {
        GameObject projectile = Instantiate(enemyProjectile, transform.position, Quaternion.identity);

        projectile.transform.parent = GetComponent<Enemy>().target.transform;

        projectile.GetComponent<Enemy>().target = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>().enemyParent;
        projectile.GetComponent<Enemy>().speed = enemyProjectileSpeed;
        projectile.GetComponent<Enemy>().damage = projectileDamage;

        Destroy(projectile, 60f);
    }
}
