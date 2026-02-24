using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MoneyMultiplierEvent", menuName = "ScriptableObjects/Events")]
public class MoneyMultiplierEvent : ScriptableObject, IEvent
{
    public float moneyIncrease;
    public float duration;

    [Space(10)]

    public bool enabled = true;

    public void ApplyEvent()
    {
        EventManager.Instance.StartCoroutine(Event(moneyIncrease, duration));
    }

    public bool IsEnabled()
    {
        return enabled;
    }

    IEnumerator Event(float multiplier, float duration)
    {
        EventManager eventManager = EventManager.Instance;
        MoneyManager moneyManager = MoneyManager.Instance;

        eventManager.StartCoroutine(eventManager.EnableEventText("x" + multiplier + " Money Multiplier", duration));
        eventManager.eventHappening = true;

        moneyManager.eventMoneyMultiplier *= multiplier;

        yield return new WaitForSeconds(duration);

        moneyManager.eventMoneyMultiplier = 1f;
        eventManager.eventHappening = false;
    }
}
