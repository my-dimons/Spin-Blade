using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    PersistentVariables persistentVariables;
    public GameObject timeText;
    public GameObject killsText;
    public GameObject totalMoneyText;

    public GameObject totalTimeText;

    [Header("Stats")]

    public int lShiftPresses = 0;
    public int kills = 0;
    public float totalMoneyGained;
    public float totalBitsGained;
    float totalTimePlayed;

    private void Start()
    {
        persistentVariables = GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>();
        foreach (Slider sfx in sfxSliders)
        {
            sfx.onValueChanged.AddListener(OnSfxSliderValueChanged);
            sfx.value = persistentVariables.sfxVolume;
        }
        foreach (Slider music in musicSliders)
        {
            music.onValueChanged.AddListener(OnMusicSliderValueChanged);
            music.value = persistentVariables.musicVolume;
        }

        if (tutorialText != null && !persistentVariables.infiniteMode)
            tutorialText.text = tutorialStrings[tutorialStage];
        else if (persistentVariables.infiniteMode)
            tutorialText.text = "";
            Time.timeScale = 1; // Ensure the game is running at normal speed


    }
    private void Update()
    {
        totalTimePlayed += Time.deltaTime;

        if (persistentVariables.infiniteMode)
        {
            if (!totalTimeText.activeSelf)
            {
                totalTimeText.SetActive(true);
            }

            TimeSpan timePlayed = TimeSpan.FromSeconds(Mathf.RoundToInt(totalTimePlayed));
            totalTimeText.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", timePlayed.Minutes, timePlayed.Seconds);
        }

        if (!tutorialFinished && tutorialText != null && !persistentVariables.infiniteMode)
            Tutorial();


    }

    private void Tutorial()
    {
        float money = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().money;
        MoneyManager moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();

        // skip tutorial
        if (Input.GetKeyDown(KeyCode.RightShift)) {
            tutorialFinished = true;
            tutorialText.text = "";
        }
            
        // tutorial
        if ((GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().switchKey || moneyManager.toggleShopKey) && tutorialStage == 0)
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
        else if (moneyManager.toggleShopKey && tutorialStage == 1)
        {
            AdvanceTutorial();
        }
        else if ((Input.GetMouseButtonUp(1) || Input.GetMouseButtonDown(0)) && tutorialStage == 2)
        {
            AdvanceTutorial();
        }
        else if (moneyManager.toggleShopKey && tutorialStage == 3)
        {
            AdvanceTutorial();
        }
        else if (tutorialStage == 4 && !advancedTutorialStage)
        {
            StartCoroutine(AdvanceTutorialLate(4f));
        }
        else if (GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().money >= 5 && tutorialStage == 5)
        {
            AdvanceTutorial();
        }
        else if (money < ogMoney && tutorialStage == 6)
            AdvanceTutorial();
        else if (tutorialStage == 7 && !advancedTutorialStage)
        {
            StartCoroutine(AdvanceTutorialLate(2f));
        }
        else if (tutorialStage >= tutorialStrings.Length)
        {
            tutorialText.gameObject.SetActive(false);
            tutorialFinished = true;
        }
        ogMoney = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().money;
    }

    void AdvanceTutorial(int amount = 1)
    {
        tutorialStage += amount;
        tutorialText.text = tutorialStrings[tutorialStage];
        advancedTutorialStage = false;
    }

    public void OnSfxSliderValueChanged(float value)
    {
        persistentVariables.sfxVolume = value;
        Debug.Log("Variable updated: " + persistentVariables.sfxVolume);
    }
    public void OnMusicSliderValueChanged(float value)
    {
        persistentVariables.musicVolume = value;
        Debug.Log("Variable updated: " + persistentVariables.musicVolume);
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
        persistentVariables.infiniteMode = false;
    }
    public void LoadGame(float difficulty = 1)
    {
        float easyMoneyMultiplier = 1.5f;
        float hardMoneyMultiplier = 0.7f;
        LoadScene("Gameplay");
        persistentVariables.difficulty = difficulty;
        if (difficulty < 1)
        {
            persistentVariables.moneyMultiplier = easyMoneyMultiplier;
        } else if (difficulty > 1)
        {
            persistentVariables.moneyMultiplier = hardMoneyMultiplier;
        } else
        {
            persistentVariables.moneyMultiplier = 1;
        }
    }

    public void RetryGame()
    {
        LoadScene("Gameplay");
    }

    public void LoadGameInf()
    {
        LoadScene("Gameplay");
        persistentVariables.infiniteMode = true;
        persistentVariables.difficulty = 1f;
        persistentVariables.moneyMultiplier = 1;
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
        killsText.GetComponent<TextMeshProUGUI>().text = "Kills = " + kills.ToString();

        // money
        totalMoneyText.GetComponent<TextMeshProUGUI>().text = "Gained $" + totalMoneyGained.ToString("F2");

        Debug.Log("WIN SCREEN ENABLED: " + winTime.ToString());
        yield return new WaitForSeconds(winTime);
        Debug.Log("WIN SCREEN DISABLED");
        winScreen.SetActive(false);
    }

    public void Win()
    {
        StartCoroutine(WinScreen());
    }
}
