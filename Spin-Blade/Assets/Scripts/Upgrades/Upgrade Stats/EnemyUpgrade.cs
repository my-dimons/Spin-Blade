using UnityEngine;

public class EnemyUpgrade : MonoBehaviour, IUpgrade
{
    [Header("Enemies")]
    public GameObject addEnemy; // leave null to not add any enemies to the spawning
    public float enemySpeedMultiplierIncrease;
    public float enemyDifficultyIncrease;
    public float enemySpawnRateIncrease;

    [Header("Bosses")]
    public float enemyBossHealthMultiplierIncrease;

    public void ApplyUpgrade()
    {
        EnemyManager enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

        if (addEnemy != null)
            enemyManager.enemies.Add(addEnemy);

        enemyManager.bossHealthMultiplier += enemyBossHealthMultiplierIncrease;
        enemyManager.spawnRate += enemySpawnRateIncrease;
        enemyManager.enemySpeedMultiplier += enemySpeedMultiplierIncrease;
    }
}