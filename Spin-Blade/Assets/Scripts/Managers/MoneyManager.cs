using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MoneyManager : MonoBehaviour
{
    public float money;
    public float moneyMultiplier = 1f;
    [HideInInspector()] public float eventMoneyMultiplier = 1;
    public float passiveIncome;
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

    private void Start()
    {
        shopMenuPos = skillTreeObject.GetComponent<RectTransform>().anchoredPosition;
        foreach (Transform child in upgradeParent.transform)
        {
            if (child.GetComponent<Upgrade>())
            {
                upgrades.Add(child.gameObject);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        PassiveIncome();

        // toggle shop
        KeyCode key = KeyCode.Tab;
        if (Input.GetKeyDown(key) && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().currentHealth > 0 && !animatingShop)
        {
            ToggleShop();
        }

        UpdateMoneyText();
    }

    public float CalculateMoney(float number)
    {
        return number * moneyMultiplier * eventMoneyMultiplier;
    }
    private void UpdateMoneyText()
    {
        if (money >= 1000)
            moneyText.text = "$" + money.ToString("F0");
        if (money >= 100)
            moneyText.text = "$" + money.ToString("F1");
        else
            moneyText.text = "$" + money.ToString("F2");


        string moneyMultiplierString = "";
        // money multiplier text
        if (moneyMultiplier > 1)
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

    private void ToggleShop()
    {
        Utils.PlayClip(uiSound, 0.3f);
        float animTime = 0.1f;

        if (shopMenu.activeSelf == true)
        {
            animatingShop = true;
            StartCoroutine(Utils.AnimateValue(1, .6f, animTime, upgradeInfoAnimCurve,
                value => shopMenu.transform.localScale = Vector3.one * value, useRealtime: true));
            StartCoroutine(Utils.EnableObjectDelay(shopMenu, false, animTime));
            StartCoroutine(AnimatingBoolToggle(animTime, false));
        } else
        {
            skillTreeObject.GetComponent<RectTransform>().anchoredPosition = shopMenuPos;
            animatingShop = true;
            shopMenu.SetActive(true);
            StartCoroutine(Utils.AnimateValue(.6f, 1, animTime, upgradeInfoAnimCurve,
                value => shopMenu.transform.localScale = Vector3.one * value, useRealtime: true));
            StartCoroutine(AnimatingBoolToggle(animTime, false));
        }
            Time.timeScale = Time.timeScale == 0 ? 1 : 0; // pause or unpause the game

        foreach (GameObject upgrade in upgrades)
        {
            foreach (Transform child in upgrade.transform)
            {
                if (child.CompareTag("UpgradeDsc"))
                    child.gameObject.SetActive(false);
            }
        }
    }

    public void AddMoney(float value)
    {
        money += value * moneyMultiplier * eventMoneyMultiplier;
    }

    public void PassiveIncome()
    {
        money += passiveIncome * Time.deltaTime * moneyMultiplier;
    }

    IEnumerator AnimatingBoolToggle(float time, bool enable)
    {
        yield return new WaitForSecondsRealtime(time);
        animatingShop = enable;
    }
}
