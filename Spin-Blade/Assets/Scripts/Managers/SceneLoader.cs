using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 0.5f;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneWithAnimation(string sceneName)
    {
        StartCoroutine(LoadSceneWithAnimationCoroutine(sceneName));
    }

    IEnumerator LoadSceneWithAnimationCoroutine(string sceneName)
    {
        transition.SetTrigger("Start Transition");

        yield return new WaitForSeconds(transitionTime);

        LoadScene(sceneName);
    }
}
