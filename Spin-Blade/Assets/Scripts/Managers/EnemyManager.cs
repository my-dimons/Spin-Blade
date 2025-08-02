using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public GameObject enemyParent;
    public float radius = 5f; // Adjustable spawning radius
    public float difficulty = 1f;
    public float difficultyIncrease = 0.05f;
    public float spawnRate = 1f;

    // used by events
    float eventSpawnRate = 1f;
    int eventCount = 0;

    public float enemySpeedMultiplier = 1f;

    [Header("Events")]
    public TextMeshPro eventText;
    public AudioClip eventPing;
    public bool eventHappening;
    public float eventDuration;
    public float eventCooldown;

    public float eventEnemySwarmAmount;
    public float eventMoneyMultiplierAmount;
    public float eventDifficultyIncreasePercent; // percentage (0-1)
    public GameObject eventBossPrefab;
    public float bossEventSpawnRate;
    private void Start()
    {
        StartCoroutine(SpawnEnemyLoop());
        StartCoroutine(EventLoop());
    }

    IEnumerator SpawnEnemyLoop()
    {
        yield return new WaitForSeconds(2f / spawnRate / eventSpawnRate);
        SpawnEnemy();
        StartCoroutine(SpawnEnemyLoop());
    }

    public void SpawnEnemy(GameObject enemyPrefab = null)
    {
        // Get a random angle (in radians)
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // Calculate the x and y position on the circle's edge
        Vector2 spawnPos = new Vector2(
            transform.position.x + Mathf.Cos(angle) * radius,
            transform.position.y + Mathf.Sin(angle) * radius
        );


        if (enemyPrefab == null)
        {
           enemyPrefab = GetRandomEnemy();
        }
        if (enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

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

    public void StartRandomEvent()
    {
        int randomNum = Random.Range(0, 3);
        if (randomNum != 3)
            Utils.PlayClip(eventPing, 1.25f);

        if (eventCount == 2)
        {
            StartCoroutine(DifficultyIncreaseEvent());
            eventCount = 0;
            return;
        } else
            eventCount++;

        switch (randomNum)
        {
            case 0:
                StartCoroutine(EnemySwarm(eventEnemySwarmAmount * difficulty * 2));
                break;
            case 1:
                StartCoroutine(MiniBossEvent());
                break;
            case 2:
                StartCoroutine(DifficultyIncreaseEvent());
                break;
            case 3:
                StartCoroutine(EventLoop());
                break;
        }
    }

    IEnumerator EventLoop()
    {
        yield return new WaitForSeconds(eventCooldown);
        if (!eventHappening && enemies.Count > 2)
        {
            StartRandomEvent();
        } else
        {
            StartCoroutine(EventLoop());
        }
    }

    IEnumerator EnemySwarm(float enemyAmount)
    {
        enemyAmount = Mathf.Round(enemyAmount);
        eventHappening = true;
        eventText.gameObject.SetActive(true);
        eventText.text = "Enemy Swarm Incoming";

        // Spawn a large number of enemies in a short time
        for (int i = 0; i < enemyAmount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f); // Short delay between spawns
        }

        eventHappening = false;

        eventText.text = "";
        eventText.gameObject.SetActive(false);

        StartCoroutine(EventLoop());
    }

    IEnumerator MiniBossEvent()
    {
        eventText.text = "Boss Incoming";
        eventText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5); // Short delay before the event starts
        eventHappening = true;
        eventText.text = "";
        eventText.gameObject.SetActive(false);

        SpawnEnemy(eventBossPrefab);
        eventSpawnRate *= bossEventSpawnRate;

        yield return new WaitForSeconds(35);

        eventSpawnRate = 1f;
        eventHappening = false;

        StartCoroutine(EventLoop());
    }

    IEnumerator DifficultyIncreaseEvent()
    {
        eventText.text = "Difficulty Increase";
        eventText.gameObject.SetActive(true);

        eventHappening = true;
        // Increase difficulty
        difficulty += (difficulty * 0.1f);
        yield return new WaitForSeconds(5);

        eventHappening = false;
        eventText.text = "";
        eventText.gameObject.SetActive(false);

        StartCoroutine(EventLoop());
    }
    IEnumerator MoneyMultiplierEvent(float multiplier)
    {
        eventText.text = "x" + multiplier + " Money Multiplier";
        eventText.gameObject.SetActive(true);
        eventHappening = true;

        MoneyManager moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();
        moneyManager.eventMoneyMultiplier *= multiplier;

        yield return new WaitForSeconds(eventDuration);

        moneyManager.eventMoneyMultiplier = 1f;
        eventHappening = false;
        eventText.text = "";
        eventText.gameObject.SetActive(false);
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
