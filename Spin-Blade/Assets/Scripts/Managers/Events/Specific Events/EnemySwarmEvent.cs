using System.Collections;
using Unity.Hierarchy;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySwarmEvent", menuName = "ScriptableObjects/Events/EnemySwarmEvent")]
public class EnemySwarmEvent : ScriptableObject, IEvent
{
    public float enemyAmount;
    public float enemySpawnSpeed;

    [Space(10)]

    public bool enabled = true;
    public string eventName = "EnemySwarmEvent";
    public string eventPopup = "Enemy Swarm Incoming!";

    public void ApplyEvent()
    {
        EventManager.Instance.StartCoroutine(Event(CalculateEnemySwarmAmount(enemyAmount), enemySpawnSpeed));
    }

    public bool IsEnabled()
    {
        return enabled;
    }

    public string GetEventName()
    {
        return eventName;
    }

    public string GetEventPopupTextString()
    {
        return eventPopup;
    }

    IEnumerator Event(float enemyAmount, float spawnDelay)
    {
        EventManager eventManager = EventManager.Instance;
        EnemyManager enemyManager = EnemyManager.Instance;
        
        eventManager.StartCoroutine(eventManager.EnableEventText(GetEventPopupTextString(), eventManager.DEFAULT_EVENT_TEXT_APPEAR_TIME));
        eventManager.eventHappening = true;

        enemyAmount = Mathf.Round(enemyAmount);

        // Spawn a large number of enemies in a short time
        for (int i = 0; i < enemyAmount; i++)
        {
            enemyManager.SpawnEnemy(enemyManager.GetRandomSpawnableEnemy());
            yield return new WaitForSeconds(enemySpawnSpeed); // Short delay between spawns
        }

        eventManager.eventHappening = false;
    }

    private float CalculateEnemySwarmAmount(float amount)
    {
        return amount;
    }
}
