using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public float money;
    public float moneyMultiplier = 1f;
    [HideInInspector()] public float eventMoneyMultiplier = 1;
    public float passiveIncome;
    public GameObject infModePauseMenu;
    public GameObject shopMenu;
    public GameObject skillTreeObject;
    public Vector2 shopMenuPos;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI moneyMultiplierText;
    public TextMeshProUGUI moneyPerSecondText;
    public Vector2 moneyPerSecondTextPos;
    public GameObject upgradeParent;
    private List<GameObject> upgrades = new List<GameObject>();
    public AudioClip uiSound;

    [Header("Ui Animations")]
    public AnimationCurve upgradeInfoAnimCurve;
    private bool animatingShop;

    [HideInInspector] public bool toggleShopKey;
    private bool firstShopToggle;
    private void Start()
    {
        moneyMultiplier *= GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().moneyMultiplier;
        shopMenuPos = skillTreeObject.GetComponent<RectTransform>().anchoredPosition;
        foreach (Transform child in upgradeParent.transform)
        {
            if (child.GetComponent<Upgrade>())
            {
                upgrades.Add(child.gameObject);
            }
        }

        InvokeRepeating("PassiveIncome", 0, 1);
    }
    // Update is called once per frame
    void Update()
    {
        toggleShopKey = Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2);

        if (toggleShopKey && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().currentHealth > 0 && !animatingShop)
        {
            if (!GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
                ToggleShop(shopMenu);
            else 
                ToggleShop(infModePauseMenu);
        }

        UpdateMoneyText();
        money = Mathf.Round(money * 100f) / 100f;
    }

    public float CalculateMoney(float number)
    {
        return number * moneyMultiplier * eventMoneyMultiplier;
    }
    private void UpdateMoneyText()
    {
        string moneyString = "";
        if (money >= 1000)
            moneyString = "$" + money.ToString("F0");
        if (money >= 100)
            moneyString = "$" + money.ToString("F1");
        else
            moneyString = "$" + money.ToString("F2");
        if (GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
            moneyString = "";
        moneyText.text = moneyString;

        string moneyMultiplierString = "";
        // money multiplier text
        if (moneyMultiplier != 1)
        {
            moneyMultiplierString = "x" + moneyMultiplier.ToString("F2");
        }
        
        if (eventMoneyMultiplier > 1)
        {
            moneyMultiplierText.text = moneyMultiplierString + " (x" + eventMoneyMultiplier.ToString("F0") + ")";
        }

        moneyMultiplierText.text = moneyMultiplierString;

        // money per second text
        string moneyPerSecondString = "";
        if (passiveIncome > 0)
        {
            moneyPerSecondString = "+" + passiveIncome.ToString("F1") + "/s";
        }
        moneyPerSecondText.text = moneyPerSecondString;

        string text = moneyText.text;
        int anchorIndex = text.Length - 1;

        if (anchorIndex < 0 || anchorIndex >= moneyText.textInfo.characterCount) return;

        var charInfo = moneyText.textInfo.characterInfo[anchorIndex];

        Vector3 worldPos = (charInfo.topRight + charInfo.bottomRight) / 2f;
        worldPos = moneyText.transform.TransformPoint(worldPos);

        moneyPerSecondText.rectTransform.position = worldPos + (Vector3)moneyPerSecondTextPos;
    }

    private void ToggleShop(GameObject menu)
    {
        Utils.PlayClip(uiSound, 0.3f);
        float animTime = 0.1f;

        if (menu.activeSelf == true)
        {
            animatingShop = true;
            StartCoroutine(Utils.AnimateValue(1, .6f, animTime, upgradeInfoAnimCurve,
                value => menu.transform.localScale = Vector3.one * value, useRealtime: true));
            StartCoroutine(Utils.EnableObjectDelay(menu, false, animTime));
            StartCoroutine(AnimatingBoolToggle(animTime, false));
        } else
        {
            menu.SetActive(true);

            if (firstShopToggle == false)
            {
                GetUpgradePostcursors();
                firstShopToggle = true;
            }

            animatingShop = true;
            StartCoroutine(Utils.AnimateValue(.6f, 1, animTime, upgradeInfoAnimCurve,
                value => menu.transform.localScale = Vector3.one * value, useRealtime: true));
            StartCoroutine(AnimatingBoolToggle(animTime, false));
            if (!GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
                skillTreeObject.GetComponent<DraggableSkillTreeMenu>().ResetPosition();
        }
            Time.timeScale = Time.timeScale == 0 ? 1 : 0; // pause or unpause the game

        if (!GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
        {
            foreach (GameObject upgrade in upgrades)
            {
                foreach (Transform child in upgrade.transform)
                {
                    if (child.CompareTag("UpgradeDsc"))
                        child.gameObject.SetActive(false);
                    // reset to default size
                    if (child.CompareTag("Upgrade"))
                        child.gameObject.transform.localScale = Vector3.one;
                }
            }
        }
    }

    public void AddMoney(float value)
    {
        float moneyGain = value * moneyMultiplier * eventMoneyMultiplier;
        money += moneyGain;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().totalMoneyGained += moneyGain;
    }

    public void PassiveIncome()
    {
        if (Time.timeScale > 0)
            money += passiveIncome;
    }

    IEnumerator AnimatingBoolToggle(float time, bool enable)
    {
        yield return new WaitForSecondsRealtime(time);
        animatingShop = enable;
    }

    void GetUpgradePostcursors()
    {
        foreach (GameObject var in GameObject.FindGameObjectsWithTag("Upgrade"))
        {
            foreach (GameObject precursor in var.GetComponent<Upgrade>().skillTreePrecursors)
            {
                precursor.GetComponent<Upgrade>().skillTreePostcursors.Add(var);
            }
        }
    }
}
