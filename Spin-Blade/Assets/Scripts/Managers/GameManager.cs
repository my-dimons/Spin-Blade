using System.Collections;
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public string[] tutorialStrings;
    public int tutorialStage = 0;
    bool advancedTutorialStage = false;
    bool tutorialFinished = false;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    public Slider sfxSlider;
    public Slider musicSlider;

    [Header("Music Settings")]
    public AudioSource musicSource;
    public AudioClip[] musicTracks;

    private int currentTrackIndex = 0;

    public int lShiftPresses = 0;
    public int kills = 0;

    private void Start()
    {
        sfxSlider.onValueChanged.AddListener(OnSfxSliderValueChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);

        tutorialText.text = tutorialStrings[tutorialStage];
        Time.timeScale = 1; // Ensure the game is running at normal speed

        if (musicTracks.Length > 0)
        {
            // Start the continuous music loop
            StartCoroutine(PlayMusicContinuously());
        }
    }
    private void Update()
    {
        if (!tutorialFinished)
            Tutorial();

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    private IEnumerator PlayMusicContinuously()
    {
        while (true)
        {
            // Pick the current track
            AudioClip clip = musicTracks[currentTrackIndex];
            musicSource.clip = clip;
            musicSource.Play();

            // Wait until the current track is finished
            yield return new WaitForSecondsRealtime(clip.length);

            // Go to the next track (wrap around)
            currentTrackIndex = Random.Range(0, musicTracks.Length);
            StartCoroutine(PlayMusicContinuously());
        }
    }

    private void Tutorial()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && tutorialStage == 0)
        {
            AdvanceTutorial();
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && tutorialStage == 1)
        {
            AdvanceTutorial();
        }
        else if (Input.GetMouseButtonUp(1) && tutorialStage == 2)
        {
            AdvanceTutorial();
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && tutorialStage == 3)
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
        else if (GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>().money < 5 && tutorialStage == 6)
        {
            AdvanceTutorial();
        }
        else if (tutorialStage == 7 && !advancedTutorialStage)
        {
            StartCoroutine(AdvanceTutorialLate(2f));
        }
        else if (tutorialStage >= tutorialStrings.Length)
        {
            tutorialText.gameObject.SetActive(false);
            tutorialFinished = true;
        }
    }

    void AdvanceTutorial()
    {
        tutorialStage++;
        tutorialText.text = tutorialStrings[tutorialStage];
        advancedTutorialStage = false;
    }

    public void OnSfxSliderValueChanged(float value)
    {
        sfxVolume = value;
        Debug.Log("Variable updated: " + sfxVolume);
    }
    public void OnMusicSliderValueChanged(float value)
    {
        musicVolume = value;
        Debug.Log("Variable updated: " + musicVolume);
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
    public void LoadGame()
    {
        LoadScene("Gameplay");
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
}
