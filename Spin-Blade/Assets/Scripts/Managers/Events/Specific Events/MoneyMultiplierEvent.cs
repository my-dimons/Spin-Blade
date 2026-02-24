using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MoneyMultiplierEvent", menuName = "ScriptableObjects/Events/MoneyMultiplierEvent")]
public class MoneyMultiplierEvent : Event
{
    public float moneyIncrease;
    public float duration;

    [Space(10)]

    public bool enabled = true;
    public string eventName = "MoneyMultiplierEvent";
    public string eventPopup = " Money Multiplier";

    public override void ApplyEvent()
    {
        EventManager.Instance.StartCoroutine(Event(moneyIncrease, duration));
    }

    public override bool IsEnabled()
    {
        return enabled;
    }

    public override string GetEventName()
    {
        return eventName;
    }

    public override string GetEventPopupTextString()
    {
        return "x" + moneyIncrease + eventPopup;
    }

    IEnumerator Event(float multiplier, float duration)
    {
        EventManager eventManager = EventManager.Instance;
        MoneyManager moneyManager = MoneyManager.Instance;

        eventManager.StartCoroutine(eventManager.EnableEventText(GetEventPopupTextString(), duration));
        eventManager.eventHappening = true;

        moneyManager.eventMoneyMultiplier *= multiplier;

        yield return new WaitForSeconds(duration);

        moneyManager.eventMoneyMultiplier = 1f;
        eventManager.eventHappening = false;
    }
}
