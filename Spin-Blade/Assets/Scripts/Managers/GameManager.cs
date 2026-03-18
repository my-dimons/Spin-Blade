using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityUtils.ScriptUtils.Audio;

public class GameManager : MonoBehaviour {
  [Header("Volume Settings")]
  public Slider[] sfxSliders;
  public Slider[] musicSliders;

  [Header("CRT Shader Settings")]
  public Button crtToggle;
  public GameObject crtToggleCheckmark;
  [Space(10)]
  public FullScreenPassRendererFeature crtRendererFeature;
  public Material crtMaterial;
  public Material defaultMaterial;

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

    if (crtToggle != null) {
      crtToggle.onClick.AddListener(() => {
        UpdateCRTEnabled(!difficultyVariables.crtEnabled);
      });
    }
  }

  private void Update() {
    totalTimePlayed += Time.deltaTime;

    if (difficultyVariables.crtEnabled) {
      if (crtMaterial != null)
        crtRendererFeature.passMaterial = crtMaterial;
    } else {
      if (defaultMaterial != null)
        crtRendererFeature.passMaterial = defaultMaterial;
    }
  }

  public void LoadMenu() {
    SceneLoader.GetInstance().LoadSceneWithAnimation("Menu");
    Time.timeScale = 1;
  }

  public void UpdateCRTEnabled(bool enable) {
    difficultyVariables.crtEnabled = enable;
    crtToggleCheckmark.SetActive(enable);
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
}
