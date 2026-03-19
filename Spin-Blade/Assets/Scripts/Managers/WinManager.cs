using UnityEngine;

public static class WinManager {
	public const int WIN_FRAGMENTS_NEEDED = 3;
	public static bool won = false;
	public static int winFragements { get; private set; }

	public static void AddWinFragment(int amount) {
		winFragements += amount;
		if (winFragements >= WIN_FRAGMENTS_NEEDED) {
			GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().Win();
			won = true;
		}
	}

	[RuntimeInitializeOnLoadMethod]
	public static void InitializeWinManager() {
		winFragements = 0;
	}
}
