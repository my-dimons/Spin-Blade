using UnityEngine;

public class EnemyUpgrade : MonoBehaviour, IUpgrade
{
    [Header("Enemies")]
    public Enemy addEnemy;
    public float enemySpeedMultiplierIncrease;
    public float enemyDifficultyIncrease;
    public float enemySpawnRateIncrease;

    [Header("Bosses")]
    public float enemyBossHealthMultiplierIncrease;

    public void ApplyUpgrade()
    {
        EnemyManager enemyManager = EnemyManager.Instance;

        if (addEnemy != null)
            enemyManager.enemies.Add(addEnemy);

        enemyManager.bossHealthMultiplier += enemyBossHealthMultiplierIncrease;
        enemyManager.spawnRate += enemySpawnRateIncrease;
        enemyManager.enemySpeedMultiplier += enemySpeedMultiplierIncrease;
    }
}