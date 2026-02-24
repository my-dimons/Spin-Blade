using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyIncreaseEvent", menuName = "ScriptableObjects/Events")]
public class DifficultyIncreaseEvent : ScriptableObject, IEvent
{
    public float difficultyIncreasePercent;
    public float duration;

    [Space(10)]

    public bool enabled = true;

    public void ApplyEvent()
    {
        Event(difficultyIncreasePercent, duration);
    }

    public bool IsEnabled()
    {
        return enabled;
    }

    void Event(float multiplier, float duration)
    {
        EventManager eventManager = EventManager.Instance;
        EnemyManager enemyManager = EnemyManager.Instance;

        eventManager.StartCoroutine(eventManager.EnableEventText("Difficulty Increase!", eventManager.DEFAULT_EVENT_TEXT_APPEAR_TIME));
        eventManager.eventHappening = true;

        EnemyManager.Instance.IncreaseDifficulty(enemyManager.difficulty * (difficultyIncreasePercent / 100));

        eventManager.eventHappening = false;
    }
}
