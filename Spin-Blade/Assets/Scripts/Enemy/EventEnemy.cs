using UnityEngine;

[RequireComponent(typeof(Enemy))] 
public class EventEnemy : MonoBehaviour
{
    void TriggerEvent()
    {
        GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>().StartRandomEvent();
    }

    private void OnEnable()
    {
        GetComponent<Enemy>().OnDeath += TriggerEvent;
    }

    private void OnDisable()
    {
        GetComponent<Enemy>().OnDeath -= TriggerEvent;
    }
}
