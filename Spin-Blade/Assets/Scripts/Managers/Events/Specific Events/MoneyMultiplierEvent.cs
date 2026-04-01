using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MoneyMultiplierEvent", menuName = "ScriptableObjects/Events/MoneyMultiplierEvent")]
public class MoneyMultiplierEvent : Event {
  public float moneyIncrease;
  public float duration;

  [Space(10)]

  public bool enabled = true;
  public string eventName = "MoneyMultiplierEvent";

  public override void ApplyEvent() {
    EventManager.Instance.StartCoroutine(Event(moneyIncrease, duration));
  }

  public override bool IsEnabled() {
    return enabled;
  }

  public override string GetEventName() {
    return eventName;
  }

  public override string GetEventPopupTextString() {
    return "+" + moneyIncrease + "x Money Multiplier";
  }

  IEnumerator Event(float moneyMultiplier, float duration) {
    EventManager eventManager = EventManager.Instance;
    MoneyManager moneyManager = MoneyManager.Instance;

    eventManager.StartCoroutine(eventManager.EnableEventText(GetEventPopupTextString(), duration));
    eventManager.eventHappening = true;

    moneyManager.moneyMultiplier += moneyMultiplier;

    yield return new WaitForSeconds(duration);

    moneyManager.moneyMultiplier -= moneyMultiplier;
    eventManager.eventHappening = false;
  }
}
