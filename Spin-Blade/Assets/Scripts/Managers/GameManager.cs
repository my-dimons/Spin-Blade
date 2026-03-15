using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils.ScriptUtils.Audio;

public class GameManager : MonoBehaviour {

	[Header("Tutorial")]
	public TextMeshProUGUI tutorialText;
	public Upgrade firstUpgrade;
	[TextArea]
	public string[] tutorialStrings;
	public int tutorialStage = 0;

	private bool advancedTutorialStage = false;
	private bool tutorialFinished = false;
	private bool advanceStage3 = false;
	// used for tutorial
	float ogMoney;

	[Header("Volume Settings")]
	public Slider[] sfxSliders;
	public Slider[] musicSliders;

	[Header("Win Screen")]
	public GameObject winScreen;
	public float winTime; // how long the win screen is up for
	DifficultyVariables difficultyVariables;
	public GameObject timeText;
	public GameObject killsText;
	public GameObject totalMoneyText;

	public GameObject totalTimeText;

	[Space(5)]

	public AudioClip winSfx;

	[Header("Stats")]

	public int lShiftPresses = 0;
	public int kills = 0;
	public float totalMoneyGained;
	public float totalBitsGained;
	float totalTimePlayed;

	private void Start() {
		difficultyVariables = DifficultyVariables.Instance;

		if (tutorialText != null)
			tutorialText.text = tutorialStrings[tutorialStage];
	}
	private void Update() {
		totalTimePlayed += Time.deltaTime;

		if (!tutorialFinished && tutorialText != null)
			Tutorial();
	}

	private void Tutorial() {
		MoneyManager moneyManager = MoneyManager.Instance;
		float money = moneyManager.money;

		switch (tutorialStage) {
			case 0:
				if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().switchKey || moneyManager.toggleShopKey) {
					if (moneyManager.toggleShopKey)
						AdvanceTutorial(2);
					else
						AdvanceTutorial();
				}
				break;

			case 1:
				if (moneyManager.shopOpen)
					AdvanceTutorial();
				break;

			case 2:
				if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonDown(0))
					AdvanceTutorial();
				else if (!moneyManager.shopOpen)
					AdvanceTutorial();
				break;

			case 3:
				if (advanceStage3)
					AdvanceTutorial();
				break;

			case 4:
				if (!advancedTutorialStage)
					StartCoroutine(AdvanceTutorialLate(4f));
				break;

			case 5:
				if (MoneyManager.Instance.money >= 5)
					AdvanceTutorial();
				break;

			case 6:
				if (money < ogMoney)
					AdvanceTutorial();
				break;

			case 7:
				if (!advancedTutorialStage)
					StartCoroutine(AdvanceTutorialLate(2f));
				break;
			default:
				break;
		}

		ogMoney = MoneyManager.Instance.money;
	}

	private void OnBuyFirstUpgrade()
	{
		advanceStage3 = true;
	}

	private void AdvanceTutorial(int amount = 1) {
		tutorialStage += amount;
		tutorialText.text = tutorialStrings[tutorialStage];
		advancedTutorialStage = false;
	}

	IEnumerator AdvanceTutorialLate(float duration) {
		advancedTutorialStage = true;
		yield return new WaitForSecondsRealtime(duration);
		AdvanceTutorial();
	}
	public void LoadMenu() {
		SceneLoader.GetInstance().LoadSceneWithAnimation("Menu");
		Time.timeScale = 1;
	}
	public void LoadGame(float difficulty = 1) {
		float easyMoneyMultiplier = 1.5f;
		float hardMoneyMultiplier = 0.7f;

		SceneLoader.GetInstance().LoadSceneWithAnimation("Gameplay");

		difficultyVariables.difficulty = difficulty;
		if (difficulty < 1) {
			difficultyVariables.moneyMultiplier = easyMoneyMultiplier;
		} else if (difficulty > 1) {
			difficultyVariables.moneyMultiplier = hardMoneyMultiplier;
		} else {
			difficultyVariables.moneyMultiplier = 1;
		}
	}

	public void RetryGame() {
		SceneLoader.GetInstance().LoadSceneWithAnimation("Gameplay");
	}

	public void LoadDifficulty() {
		SceneLoader.GetInstance().LoadSceneWithAnimation("Difficulty Selector");
	}

	public void QuitGame() {
		Debug.Log("Quitting game...");
		Application.Quit();
	}

	public IEnumerator WinScreen() {
		winScreen.SetActive(true);

		// time played
		TimeSpan timePlayed = TimeSpan.FromSeconds(Mathf.RoundToInt(totalTimePlayed));
		timeText.GetComponent<TextMeshProUGUI>().text = "Time: " + string.Format("{0:00}:{1:00}", timePlayed.Minutes, timePlayed.Seconds);

		// kills
		killsText.GetComponent<TextMeshProUGUI>().text = $"Kills: {kills}";
		// money
		totalMoneyText.GetComponent<TextMeshProUGUI>().text = "Gained: $" + totalMoneyGained.ToString("F2");

		Debug.Log("WIN SCREEN ENABLED");

		yield return new WaitForSeconds(winTime);

		Debug.Log("WIN SCREEN DISABLED");
		winScreen.SetActive(false);
	}

	[ContextMenu("Win")]
	public void Win() {
		StartCoroutine(WinScreen());
		SfxManager.PlaySfxAudioClip(winSfx, 1f);
	}

    private void OnEnable()
    {
        firstUpgrade.OnBuyUpgrade += OnBuyFirstUpgrade;
    }

    private void OnDisable()
    {
        firstUpgrade.OnBuyUpgrade -= OnBuyFirstUpgrade;
    }
}
