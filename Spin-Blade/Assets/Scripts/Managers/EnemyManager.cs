using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public GameObject enemyParent;
    public float radius = 5f; // Adjustable spawning radius
    public float difficulty = 1f;
    public float difficultyIncrease = 0.05f;

    public float enemySpeedMultiplier = 1f;
    private void Start()
    {
        StartCoroutine(SpawnEnemyLoop());
    }

    IEnumerator SpawnEnemyLoop()
    {
        yield return new WaitForSeconds(2f / difficulty);
        SpawnEnemy();
        StartCoroutine(SpawnEnemyLoop());
    }
    public void SpawnEnemy()
    {
        // Get a random angle (in radians)
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // Calculate the x and y position on the circle's edge
        Vector2 spawnPos = new Vector2(
            transform.position.x + Mathf.Cos(angle) * radius,
            transform.position.y + Mathf.Sin(angle) * radius
        );

        GameObject prefab = GetRandomEnemy();
        if (prefab != null)
        {
            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

            enemy.transform.parent = enemyParent.transform;
            enemy.GetComponent<Enemy>().target = enemyParent;
        } else
        {
            Debug.LogWarning("No enemy prefab found to spawn.");
        }

    }
    
    public GameObject GetRandomEnemy()
    {
        List<GameObject> spawnableEnemies = new List<GameObject>();
        foreach (GameObject enemy in enemies)
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
            Debug.LogWarning("No enemies available to spawn, spawning defualt enemy");
            return null;
        }
        return spawnableEnemies[Random.Range(0, spawnableEnemies.Count)];
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Circle color
        Gizmos.DrawWireSphere(transform.position, radius); // Draw the wireframe circle
    }

    public void IncreaseDifficulty()
    {
        difficulty += difficultyIncrease;
    }
}
