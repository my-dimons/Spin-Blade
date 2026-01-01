using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityUtils.ScriptUtils.Audio;

public class EventManager : MonoBehaviour
{
    public enum Event
    {
        None,
        EnemySwarm,
        Boss,
        DifficultyIncrease
    }

    [Header("Event Setup")]
    public TextMeshPro eventText;
    public AudioClip eventPing;

    [Header("Events")]
    public bool eventHappening;
    public float eventDuration;
    public float eventCooldown;

    [Header("Enemy Swarm Event")]
    public float eventEnemySwarmAmount;

    [Header("Boss Event")]
    public Enemy[] eventBossPrefabs;

    [Header("Money Increase Event")]
    public float eventMoneyMultiplierAmount;

    [Header("Difficulty Increase Event")]
    [Tooltip("% to increase the difficulty during the increase difficulty event")]
    public float difficultyIncreasePercentEvent; 

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

    public void StartSpecificEvent(Event selectedEvent)
    {
        float enemySwarmAmount = eventEnemySwarmAmount * enemyManager.difficulty * 1.7f;

        // Sfx
        if (selectedEvent != Event.None)
            SfxManager.PlaySfxAudioClip(eventPing, 0.7f);

        // Start event
        switch (selectedEvent)
        {
            case Event.None:
                break;
            case Event.EnemySwarm:
                StartCoroutine(EnemySwarm(enemySwarmAmount));
                break;
            case Event.Boss:
                StartCoroutine(BossEvent());
                break;
            case Event.DifficultyIncrease:
                DifficultyIncreaseEvent();
                break;
        }
    }

    public Event GetRandomEvent()
    {
        Array values = Enum.GetValues(typeof(Event));

        Event randomEvent = (Event)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        return randomEvent;
    }

    public void StartRandomEvent()
    {
        StartSpecificEvent(GetRandomEvent());
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

    IEnumerator BossEvent()
    {
        EnableEventText("Boss Incoming!", defaultEventTextAppearTime);
        eventHappening = true;

        enemyManager.SpawnEnemy(GetRandomBoss());

        if (GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
        {
            yield return new WaitForSeconds(20);
        }
        else
        {
            yield return new WaitForSeconds(35);
        }

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

    private Enemy GetRandomBoss()
    {
        int enemyInt = UnityEngine.Random.Range(0, eventBossPrefabs.Length);
        Enemy enemy = eventBossPrefabs[enemyInt];

        return enemy;
    }
}