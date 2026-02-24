using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityUtils.ScriptUtils.Audio;

public class EventManager : MonoBehaviour
{
    public IEvent[] events;

    [Header("Event Setup")]
    public TextMeshPro eventText;
    public AudioClip eventPing;

    [Header("Events")]
    public bool enableEvents;
    public bool eventHappening;

    [Space(10)]

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

    public readonly float DEFAULT_EVENT_TEXT_APPEAR_TIME = 5;

    EnemyManager enemyManager;
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        enemyManager = EnemyManager.Instance;

        StartCoroutine(EventLoop());
    }

    public void StartSpecificEvent(IEvent selectedEvent)
    {
        SfxManager.PlaySfxAudioClip(eventPing, 0.7f);

        // Start event
        if (selectedEvent.IsEnabled())
        {
            selectedEvent.ApplyEvent();

            Debug.Log("Started event: " + selectedEvent.GetEventName());
        }
    }

    public IEvent GetRandomEvent()
    {
        int randInt = UnityEngine.Random.Range(0, events.Length);

        IEvent randomEvent = events[randInt];

        return randomEvent;
    }

    public void StartRandomEvent()
    {
        StartSpecificEvent(GetRandomEvent());
    }

    IEnumerator EventLoop()
    {
        yield return new WaitForSeconds(eventCooldown);
        
        if (enableEvents)
            StartRandomEvent();

        StartCoroutine(EventLoop());
    }

    public IEnumerator EnableEventText(string text, float eventTextTime)
    {
        eventText.text = text;
        eventText.gameObject.SetActive(true);

        yield return new WaitForSeconds(eventTextTime);

        eventText.text = "";
        eventText.gameObject.SetActive(false);
    }
}