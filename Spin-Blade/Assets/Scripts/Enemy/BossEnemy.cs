using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class BossEnemy : MonoBehaviour {
  [Tooltip("% of player health to take")]
  public float damagePercent = 0.7f;

  public float hitsToKill = 3;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    Enemy enemy = GetComponent<Enemy>();
    PlayerHealthAndDamage playerHealthAndDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();
    EnemyManager enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

    enemy.maxHealth = CalculateMaxHealth();
    enemy.damage = CalculateDamage();

    enemy.currentHealth = enemy.maxHealth;
  }

  public float CalculateMaxHealth() {
    PlayerHealthAndDamage playerHealthAndDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();
    EnemyManager enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

    return playerHealthAndDamage.damage * hitsToKill * enemyManager.bossHealthMultiplier;
  }

  public float CalculateDamage() {
    PlayerHealthAndDamage playerHealthAndDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

    return playerHealthAndDamage.maxHeath * damagePercent;
  }
}
