using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
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
  private float ogMoney;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

    if (tutorialText != null)
      tutorialText.text = tutorialStrings[tutorialStage];
  }

  // Update is called once per frame
  void Update() {
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

  private void OnBuyFirstUpgrade() {
    advanceStage3 = true;
  }
  private void OnEnable() {
    firstUpgrade.OnBuyUpgrade += OnBuyFirstUpgrade;
  }

  private void OnDisable() {
    firstUpgrade.OnBuyUpgrade -= OnBuyFirstUpgrade;
  }
}
