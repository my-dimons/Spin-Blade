using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityUtils.ScriptUtils.Audio;

public class GameManager : MonoBehaviour
{

    public TextMeshProUGUI tutorialText;
    [TextArea]
    public string[] tutorialStrings;
    public int tutorialStage = 0;
    bool advancedTutorialStage = false;
    bool tutorialFinished = false;    
    
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

    private void Start()
    {
        difficultyVariables = DifficultyVariables.Instance;

        if (tutorialText != null)
            tutorialText.text = tutorialStrings[tutorialStage];
    }
    private void Update()
    {
        totalTimePlayed += Time.deltaTime;

        if (!tutorialFinished && tutorialText != null)
            Tutorial();
    }

    private void Tutorial()
    {
        MoneyManager moneyManager = MoneyManager.Instance;
        float money = moneyManager.money;

        // skip tutorial
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            tutorialFinished = true;
            tutorialText.text = "";
        }
            
        // tutorial
        if (tutorialStage >= tutorialStrings.Length)
        {
            tutorialText.gameObject.SetActive(false);
            tutorialFinished = true;
        }
        else if ((GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().switchKey || moneyManager.toggleShopKey) && tutorialStage == 0)
        {
            if (moneyManager.toggleShopKey)
            {
                AdvanceTutorial(2);
            }
            else
            {
                AdvanceTutorial();
            }
        }
        else if (moneyManager.shopOpen && tutorialStage == 1)
        {
            AdvanceTutorial();
        }
        else if ((Input.GetMouseButtonUp(1) || Input.GetMouseButtonDown(0)) && tutorialStage == 2)
        {
            AdvanceTutorial();
        }
        else if (!moneyManager.shopOpen && (tutorialStage == 3 || tutorialStage == 2))
        {
            AdvanceTutorial();
        }
        else if (tutorialStage == 4 && !advancedTutorialStage)
        {
            StartCoroutine(AdvanceTutorialLate(4f));
        }
        else if (MoneyManager.Instance.money >= 5 && tutorialStage == 5)
        {
            AdvanceTutorial();
        }
        else if (money < ogMoney && tutorialStage == 6)
            AdvanceTutorial();
        else if (tutorialStage == 7 && !advancedTutorialStage)
        {
            StartCoroutine(AdvanceTutorialLate(2f));
        }
        ogMoney = MoneyManager.Instance.money;
    }

    void AdvanceTutorial(int amount = 1)
    {
        tutorialStage += amount;
        tutorialText.text = tutorialStrings[tutorialStage];
        advancedTutorialStage = false;
    }

    IEnumerator AdvanceTutorialLate(float duration)
    {
        advancedTutorialStage = true;
        yield return new WaitForSecondsRealtime(duration);
        AdvanceTutorial();
    }
    public void LoadMenu()
    {
        LoadScene("Menu");
    }
    public void LoadGame(float difficulty = 1)
    {
        float easyMoneyMultiplier = 1.5f;
        float hardMoneyMultiplier = 0.7f;
        LoadScene("Gameplay");
        difficultyVariables.difficulty = difficulty;
        if (difficulty < 1)
        {
            difficultyVariables.moneyMultiplier = easyMoneyMultiplier;
        } else if (difficulty > 1)
        {
            difficultyVariables.moneyMultiplier = hardMoneyMultiplier;
        } else
        {
            difficultyVariables.moneyMultiplier = 1;
        }
    }

    public void RetryGame()
    {
        LoadScene("Gameplay");
    }

    public void LoadDifficulty()
    {
        LoadScene("Difficulty Selector");
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public IEnumerator WinScreen()
    {
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
    public void Win()
    {
        StartCoroutine(WinScreen());
        SfxManager.PlaySfxAudioClip(winSfx, 1f);
    }
}
