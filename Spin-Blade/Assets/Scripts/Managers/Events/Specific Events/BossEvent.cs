using UnityEngine;

[CreateAssetMenu(fileName = "BossEvent", menuName = "ScriptableObjects/Events/BossEvent")]
public class BossEvent : Event {
	public Enemy[] bosses;

	[Space(10)]

	public bool enabled = true;
	public string eventName = "BossEvent";
	public string eventPopup = "Boss Incoming!";

	public override void ApplyEvent() {
		Event(GetRandomBoss());
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

	void Event(Enemy boss) {
		EventManager eventManager = EventManager.Instance;
		EnemyManager enemyManager = EnemyManager.Instance;

		eventManager.StartCoroutine(eventManager.EnableEventText(GetEventPopupTextString(), eventManager.DEFAULT_EVENT_TEXT_APPEAR_TIME));

		enemyManager.SpawnEnemy(boss);

		eventManager.eventHappening = false;
	}

	private Enemy GetRandomBoss() {
		int enemyInt = Random.Range(0, bosses.Length);
		Enemy enemy = bosses[enemyInt];

		return enemy;
	}
}
