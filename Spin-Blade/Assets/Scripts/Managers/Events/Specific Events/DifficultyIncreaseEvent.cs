using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyIncreaseEvent", menuName = "ScriptableObjects/Events/DifficultyIncreaseEvent")]
public class DifficultyIncreaseEvent : Event {
	public float difficultyIncreasePercent;

	[Space(10)]

	public bool enabled = true;
	public string eventName = "DifficultyIncreaseEvent";
	public string eventPopup = "Difficulty Increase!";

	public override void ApplyEvent() {
		Event(difficultyIncreasePercent);
	}

	public override bool IsEnabled() {
		return enabled;
	}

	public override string GetEventName() {
		return eventName;
	}

	public override string GetEventPopupTextString() {
		return eventPopup;
	}

	void Event(float multiplier) {
		EventManager eventManager = EventManager.Instance;
		EnemyManager enemyManager = EnemyManager.Instance;

		eventManager.StartCoroutine(eventManager.EnableEventText(GetEventPopupTextString(), eventManager.DEFAULT_EVENT_TEXT_APPEAR_TIME));
		eventManager.eventHappening = true;

		EnemyManager.Instance.IncreaseDifficulty(enemyManager.difficulty * (difficultyIncreasePercent / 100));

		eventManager.eventHappening = false;
	}
}
