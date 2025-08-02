using System;
using System.Collections;
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    float time;

    public TextMeshProUGUI tutorialText;
    public string[] tutorialStrings;
    public int tutorialStage = 0;
    bool advancedTutorialStage = false;
    bool tutorialFinished = false;    
    
    // used for tutorial
    float ogMoney;

    [Header("Volume Settings")]
    public Slider[] sfxSliders;
    public Slider[] musicSliders;

    [Header("Music Settings")]
    public AudioSource musicSource;
    public AudioClip[] musicTracks;
    public AudioClip currentMusic;

    [Header("Win Screen")]
    public GameObject winScreen;
    public int lShiftPresses = 0;
    public int kills = 0;
    public float totalMoneyGained;
    public float winTime; // how long the win screen is up for
    PersistentVariables persistentVariables;
    public GameObject timeText;
    public GameObject killsText;
    public GameObject totalMoneyText;

    public GameObject totalTimeText;


    private void Start()
    {
        persistentVariables = GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>();
        if (sfxSliders != null)
        {
            foreach (Slider sfx in sfxSliders)
            {
                sfx.onValueChanged.AddListener(OnSfxSliderValueChanged);
                sfx.value = persistentVariables.sfxVolume;
            }

        }
        if (musicSliders != null)
        {
            foreach (Slider music in musicSliders)
            {
                music.onValueChanged.AddListener(OnMusicSliderValueChanged);
                music.value = persistentVariables.musicVolume;
            }
        }

        if (tutorialText != null && !persistentVariables.infiniteMode)
            tutorialText.text = tutorialStrings[tutorialStage];
        else if (persistentVariables.infiniteMode)
            tutorialText.text = "";
            Time.timeScale = 1; // Ensure the game is running at normal speed

        if (musicTracks.Length > 0)
            StartCoroutine(PlayMusicContinuously());
    }
    private void Update()
    {
        time += Time.deltaTime;

        if (persistentVariables.infiniteMode)
        {
            if (!totalTimeText.activeSelf)
            {
                totalTimeText.SetActive(true);
            }

            TimeSpan timePlayed = TimeSpan.FromSeconds(Mathf.RoundToInt(time));
            totalTimeText.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", timePlayed.Minutes, timePlayed.Seconds);
        }

        if (!tutorialFinished && tutorialText != null && !persistentVariables.infiniteMode)
            Tutorial();

        if (musicSource != null)
        {
            musicSource.volume = persistentVariables.musicVolume;
        }
    }

    private IEnumerator PlayMusicContinuously()
    {
        while (true)
        {
            // Pick a random track
            AudioClip clip = musicTracks[UnityEngine.Random.Range(0, musicTracks.Length)];
            musicSource.clip = clip;
            currentMusic = clip;
            musicSource.Play();

            Debug.Log($"Playing: {clip.name}");

            // Wait until the music finishes naturally
            yield return new WaitWhile(() => musicSource.isPlaying);
        }
    }

    private void Tutorial()
    {
        float money = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().money;
        MoneyManager moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();

        if ((Input.GetKeyDown(KeyCode.LeftShift) || moneyManager.toggleShopKey) && tutorialStage == 0)
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
        else if ((Input.GetMouseButtonUp(1) || Input.GetMouseButtonDown(0))&& tutorialStage == 2)
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
        float moneyMultiplierMultiplier = 1.8f;
        LoadScene("Gameplay");
        persistentVariables.difficulty = difficulty;
        if (difficulty < 1)
        {
            persistentVariables.moneyMultiplier = difficulty * moneyMultiplierMultiplier;
        } else if (difficulty > 1)
        {
            persistentVariables.moneyMultiplier = difficulty / moneyMultiplierMultiplier;
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
        TimeSpan timePlayed = TimeSpan.FromSeconds(Mathf.RoundToInt(time));
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
