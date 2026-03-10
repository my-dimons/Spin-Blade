using UnityEngine;

public class DifficultyVariables : MonoBehaviour {
	public static DifficultyVariables Instance { get; private set; }

	public float difficulty = 1;
	public float moneyMultiplier = 1;

	void Awake() {
		// Singleton pattern
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
}