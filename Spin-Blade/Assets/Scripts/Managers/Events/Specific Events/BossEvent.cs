using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BossEvent", menuName = "ScriptableObjects/Events")]
public class BossEvent : ScriptableObject, IEvent
{
    public Enemy[] bosses;

    [Space(10)]

    public bool enabled = true;

    public void ApplyEvent()
    {
        Event(GetRandomBoss());
    }

    public bool IsEnabled()
    {
        return enabled;
    }

    void Event(Enemy boss)
    {
        EventManager eventManager = EventManager.Instance;
        EnemyManager enemyManager = EnemyManager.Instance;

        eventManager.StartCoroutine(eventManager.EnableEventText("Boss Incoming!", eventManager.DEFAULT_EVENT_TEXT_APPEAR_TIME));

        enemyManager.SpawnEnemy(boss);

        eventManager.eventHappening = false;
    }

    private Enemy GetRandomBoss()
    {
        int enemyInt = Random.Range(0, bosses.Length);
        Enemy enemy = bosses[enemyInt];

        return enemy;
    }
}
