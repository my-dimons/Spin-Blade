using System.Collections;
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public string[] tutorialStrings;
    public int tutorialStage = 0;
    bool finishedTutorial = false;
    private void Start()
    {
        tutorialText.text = tutorialStrings[tutorialStage];
        Time.timeScale = 1; // Ensure the game is running at normal speed
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && tutorialStage == 0)
        {
            AdvanceTutorial();
        } else if (Input.GetKeyDown(KeyCode.Tab) && tutorialStage == 1)
        {
            AdvanceTutorial();
        } else if (Input.GetKeyDown(KeyCode.Tab) && tutorialStage == 2)
        {
            AdvanceTutorial();
        } else if (tutorialStage == 3 && !finishedTutorial)
        {
            StartCoroutine(AdvanceTutorialLate(5));
        }
    }
    void AdvanceTutorial()
    {
        tutorialStage++;
        tutorialText.text = tutorialStrings[tutorialStage];
    }
    IEnumerator AdvanceTutorialLate(float duration)
    {
        finishedTutorial = true;
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
