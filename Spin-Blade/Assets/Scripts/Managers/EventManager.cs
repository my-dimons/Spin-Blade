using System.Collections;
using TMPro;
using UnityEngine;
using UnityUtils.ScriptUtils.Audio;

public class EventManager : MonoBehaviour
{
    [Header("Event Setup")]
    public TextMeshPro eventText;
    public AudioClip eventPing;
    public bool eventHappening;
    public float eventDuration;
    public float eventCooldown;

    public float eventEnemySwarmAmount;
    public float eventMoneyMultiplierAmount;
    public Enemy eventBossPrefab;
    public float bossEventSpawnRate;

    [Header("Events")]

    [Tooltip("% to increase the difficulty during the increase difficulty event")]
    public float difficultyIncreasePercentEvent; 

    // used by events
    private float eventSpawnRate = 1f;
    private int eventCount;

    private float defaultEventTextAppearTime = 5;

    EnemyManager enemyManager;
    public static EventManager Instance { get; private set; }

    // Use this for initialization
    void Start()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);

        enemyManager = EnemyManager.Instance;

        StartCoroutine(EventLoop());
    }

    public void StartRandomEvent()
    {
        int randomNum = Random.Range(0, 2);
        if (randomNum != 2)
            SfxManager.PlaySfxAudioClip(eventPing, 0.7f);

        if (eventCount == 1)
        {
            DifficultyIncreaseEvent();
            eventCount = 0;
            return;
        }
        else
            eventCount++;

        switch (randomNum)
        {
            case 0:
                StartCoroutine(EnemySwarm(eventEnemySwarmAmount * enemyManager.difficulty * 1.7f));
                break;
            case 1:
                StartCoroutine(MiniBossEvent());
                break;
            case 2:
                StartCoroutine(EventLoop());
                break;
        }
    }

    IEnumerator EventLoop()
    {
        yield return new WaitForSeconds(eventCooldown);

        if (!eventHappening && enemyManager.enemies.Count > 2 || !eventHappening && GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
        {
            StartRandomEvent();
        }
        else
        {
            StartCoroutine(EventLoop());
        }
    }

    IEnumerator EnemySwarm(float enemyAmount)
    {
        EnableEventText("Enemy Swarm Incoming!", defaultEventTextAppearTime);
        eventHappening = true;

        enemyAmount = Mathf.Round(enemyAmount);
        // Spawn a large number of enemies in a short time
        for (int i = 0; i < enemyAmount; i++)
        {
            enemyManager.SpawnEnemy(enemyManager.GetRandomSpawnableEnemy());
            yield return new WaitForSeconds(1.5f); // Short delay between spawns
        }

        eventHappening = false;

        StartCoroutine(EventLoop());
    }

    IEnumerator MiniBossEvent()
    {
        EnableEventText("Boss Incoming!", defaultEventTextAppearTime);
        eventHappening = true;

        enemyManager.SpawnEnemy(eventBossPrefab);
        eventSpawnRate *= bossEventSpawnRate;

        if (GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
        {
            yield return new WaitForSeconds(20);
        }
        else
        {
            yield return new WaitForSeconds(35);
        }

        eventSpawnRate = 1f;
        eventHappening = false;

        StartCoroutine(EventLoop());
    }

    void DifficultyIncreaseEvent()
    {
        EnableEventText("Difficulty Increase!", defaultEventTextAppearTime);
        eventHappening = true;

        EnemyManager.Instance.IncreaseDifficulty(enemyManager.difficulty * (difficultyIncreasePercentEvent / 100));

        eventHappening = false;

        StartCoroutine(EventLoop());
    }

    IEnumerator MoneyMultiplierEvent(float multiplier, float duration)
    {
        EnableEventText("x" + multiplier + " Money Multiplier", duration);
        eventHappening = true;

        MoneyManager moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();
        moneyManager.eventMoneyMultiplier *= multiplier;

        yield return new WaitForSeconds(eventDuration);

        moneyManager.eventMoneyMultiplier = 1f;
        eventHappening = false;
    }

    IEnumerator EnableEventText(string text, float eventTextTime)
    {
        eventText.text = text;
        eventText.gameObject.SetActive(true);

        yield return new WaitForSeconds(eventTextTime);

        eventText.text = "";
        eventText.gameObject.SetActive(false);
    }
}