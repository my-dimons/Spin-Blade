using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class BossEnemy : MonoBehaviour
{
    [Tooltip("% of player health to take")]
    public float damagePercent = 1.333f;

    public float hitsToKill = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Enemy enemy = GetComponent<Enemy>();
        PlayerHealthAndDamage playerHealthAndDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();
        EnemyManager enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

        enemy.maxHealth = playerHealthAndDamage.damage * hitsToKill * enemyManager.bossHealthMultiplier;
        enemy.damage = playerHealthAndDamage.maxHeath / damagePercent;

        enemy.currentHealth = enemy.maxHealth;
    }
}
