using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class SceneLoader : MonoBehaviour {
	private readonly string exitTransitionName = "Exit_Transition";

	private static SceneLoader Instance;

	private Animator transition;

	[Tooltip("How long the transitions are. NOTE: Does not change the animation, just input how long the animation itself is")]
	public float transitionTime = 0.5f;

	private void Awake() {
		if (Instance == null) Instance = this; else Destroy(gameObject);
	}

	private void Start() {

		transition = GetComponent<Animator>();
	}

	public void LoadScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
	}

	public void LoadSceneWithAnimation(string sceneName) {
		StartCoroutine(LoadSceneWithAnimationCoroutine(sceneName));
	}

	IEnumerator LoadSceneWithAnimationCoroutine(string sceneName) {
		transition.SetTrigger(exitTransitionName);

		yield return new WaitForSeconds(transitionTime);

		//Instance = null;
		LoadScene(sceneName);
	}

	public static SceneLoader GetInstance() {
		if (Instance == null) {
			Debug.Log("ERROR, no scene loader found in scene");
			return null;
		} else {
			return Instance;
		}
	}
}
