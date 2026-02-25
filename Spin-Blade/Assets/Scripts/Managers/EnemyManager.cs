using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityUtils.ScriptUtils.Objects;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> enemies;
    public GameObject enemyParent;

    [Header("Enemy Spawning")]
    public float enemySpawningRadius = 5f; // Adjustable spawning radius
    public float difficulty = 1f;
    public float enemySpawnTimeSeconds = 1f;

    [Header("Msc Multipliers")]
    public float bossHealthMultiplier = 1f;
    public float enemySpeedMultiplier = 1f;

    public static EnemyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    private void Start()
    {
        difficulty *= DifficultyVariables.Instance.difficulty;

        StartCoroutine(SpawnEnemyLoop());
    }

    IEnumerator SpawnEnemyLoop()
    {
        while (true)
        {
            SpawnRandomSpawnableEnemy();

            yield return new WaitForSeconds(enemySpawnTimeSeconds);
        }
    }

    public void SpawnRandomSpawnableEnemy()
    {
        SpawnEnemy(GetRandomSpawnableEnemy());
    }

    public void SpawnEnemy(Enemy enemyPrefab)
    {
        // Get a random angle (in radians)
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // Calculate the x and y position on the circle's edge
        Vector2 spawnPos = new(
            transform.position.x + Mathf.Cos(angle) * enemySpawningRadius,
            transform.position.y + Mathf.Sin(angle) * enemySpawningRadius
        );

        if (enemyPrefab == null)
        {
           enemyPrefab = GetRandomSpawnableEnemy();
        }
        if (enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab.gameObject, spawnPos, Quaternion.identity);

            enemy.transform.parent = enemyParent.transform;
            enemy.GetComponent<Enemy>().target = enemyParent;
        } else
        {
            Debug.LogWarning("No enemy prefab found to spawn.");
        }
    }
    
    public Enemy GetRandomSpawnableEnemy()
    {
        List<Enemy> spawnableEnemies = new();
        foreach (Enemy enemy in enemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            float randomNum = Random.Range(0f, 1f);
            if (enemy != null && randomNum <= enemyScript.spawnRate)
            {
                spawnableEnemies.Add(enemy);
            }
        }

        if (spawnableEnemies.Count == 0)
        {
            Debug.LogWarning("No enemies available to spawn");
            return null;
        }

        return spawnableEnemies[Random.Range(0, spawnableEnemies.Count)];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Circle color
        Gizmos.DrawWireSphere(transform.position, enemySpawningRadius); // Draw the wireframe circle
    }

    public void IncreaseDifficulty(float increase)
    {
        difficulty += increase;
    }
}
