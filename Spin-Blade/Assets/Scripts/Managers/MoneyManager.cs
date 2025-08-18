using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [Header("=-- CURRENCY --=")]
    public Currency currency;
    public enum Currency
    {
        money,
        bits
    }
    public Color moneyColor;
    public Color bitsColor;

    [Header("-- Money --")]
    public float money;
    public float moneyMultiplier = 1f;
    [HideInInspector()] public float eventMoneyMultiplier = 1;
    public float passiveIncome;

    [Header("-- Bits --")]
    public float bits;
    public float bitsMultiplier = 1f;
    public bool bitsUnlocked; // just disables text rendering if off

    [Header("-- Visual Objects --")]
    public GameObject infModePauseMenu;
    public GameObject shopMenu;
    public GameObject skillTreeObject;
    public Vector2 shopMenuPos;
    [Header("Money Text")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI moneyMultiplierText;
    public TextMeshProUGUI moneyPerSecondText;
    public Vector2 moneyPerSecondTextPos;
    [Header("Bits Text")]
    public TextMeshProUGUI bitsText;
    public TextMeshProUGUI bitsMultiplierText;
    [Header("Upgrades")]
    public GameObject upgradeParent;
    private List<GameObject> upgrades = new();
    [Header("Ui Animations")]
    public AnimationCurve upgradeInfoAnimCurve;
    private bool animatingShop;

    [Header("Audio")]
    public AudioClip uiToggleSound;

    [HideInInspector] public bool toggleShopKey;
    private bool firstShopToggle;

    private void OnValidate()
    {
        moneyColor = Utils.ColorFromHex("#FFF564");
        bitsColor = Utils.ColorFromHex("#64C8FF");
    }

    private void Start()
    {
        moneyMultiplier *= GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().moneyMultiplier;
        shopMenuPos = skillTreeObject.GetComponent<RectTransform>().anchoredPosition;
        // add all upgrades to an array
        foreach (Transform child in upgradeParent.transform)
        {
            if (child.GetComponent<Upgrade>())
            {
                upgrades.Add(child.gameObject);
            }
        }

        InvokeRepeating(nameof(PassiveIncome), 0, 1);
    }
    // Update is called once per frame
    void Update()
    {
        toggleShopKey = Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2);

        if (toggleShopKey && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>().currentHealth > 0 && !animatingShop)
        {
            if (!GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
                ToggleShop(shopMenu);
            else 
                ToggleShop(infModePauseMenu);
        }

        UpdateCurrencyText();
        money = Mathf.Round(money * 100f) / 100f;
    }

    private void UpdateCurrencyText()
    {
        // money & bits text
        string bitsString = "";
        string moneyString = "";

        moneyString = CalculateMoneyString(money);
        if (bitsUnlocked)
            bitsString = CalculateMoneyString(bits, 2, Currency.bits);

        if (GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
        {
            moneyString = "";
            bitsString = "";
        }

        moneyText.text = moneyString;
        bitsText.text = bitsString;

        // money & bits multiplier text

        string moneyMultiplierString = "";
        string bitsMultiplierString = "";

        moneyMultiplierString = "x" + moneyMultiplier.ToString("F2");
        if (bitsUnlocked)
            bitsMultiplierString = "x" + bitsMultiplier.ToString("F2");

        if (eventMoneyMultiplier > 1)
            moneyMultiplierText.text = moneyMultiplierString + " (x" + eventMoneyMultiplier.ToString("F0") + ")";

        moneyMultiplierText.text = moneyMultiplierString;
        bitsMultiplierText.text = bitsMultiplierString;

        #region money per second text
        // money per second text
        string moneyPerSecondString = "";
        if (passiveIncome > 0)
        {
            moneyPerSecondString = "+" + passiveIncome.ToString("F1") + "/s";
        }
        moneyPerSecondText.text = moneyPerSecondString;

        #region money per second text anchoring
        string text = moneyText.text;
        int anchorIndex = text.Length - 1;

        if (anchorIndex < 0 || anchorIndex >= moneyText.textInfo.characterCount) return;

        var charInfo = moneyText.textInfo.characterInfo[anchorIndex];

        Vector3 worldPos = (charInfo.topRight + charInfo.bottomRight) / 2f;
        worldPos = moneyText.transform.TransformPoint(worldPos);

        moneyPerSecondText.rectTransform.position = worldPos + (Vector3)moneyPerSecondTextPos;
        #endregion
        #endregion

        #region currency text color
        moneyText.color = GetCurrencyColor(Currency.money);
        moneyMultiplierText.color = GetCurrencyColor(Currency.money);
        moneyPerSecondText.color = GetCurrencyColor(Currency.money);

        bitsText.color = GetCurrencyColor(Currency.bits);
        bitsMultiplierText.color = GetCurrencyColor(Currency.bits);
        #endregion
    }

    private void ToggleShop(GameObject menu)
    {
        Utils.PlayAudioClip(uiToggleSound, 0.3f);
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

    public float CalculateCurrency(float amount, Currency currencyType = Currency.money)
    {
        float money = amount;

        switch (currencyType)
        {
            case Currency.money:
                money *= moneyMultiplier * eventMoneyMultiplier;
                break;
            case Currency.bits:
                money *= bitsMultiplier;
                break;
        }

        return money;
    }

    public void AddCurrency(float value, Currency currencyType = Currency.money)
    {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        float currencyGain = value;
        switch (currencyType)
        {
            case Currency.money:
                currencyGain *= moneyMultiplier * eventMoneyMultiplier;
                gameManager.totalMoneyGained += currencyGain;
                money += currencyGain;
                break;
            case Currency.bits:
                currencyGain *= bitsMultiplier;
                gameManager.totalBitsGained += currencyGain;
                bits += currencyGain;
                break;
        }

        money += currencyGain;
    }

    public string CalculateMoneyString(float money, int decimalPoints = 0, Currency currencyType = Currency.money)
    {
        string moneyString = money.ToString("F2"); //todo: make decimal points do smth here

        switch (currencyType)
        {
            case Currency.money:
                moneyString = "$" + moneyString;
                break;
            case Currency.bits:
                moneyString = "(" + moneyString + ")";
                break;
        }

        return moneyString;
    }

    public Color GetCurrencyColor(Currency currenyType = Currency.money)
    {
        Color color = Color.white;

        switch(currenyType)
        {
            case Currency.money:
                color = moneyColor;
                break;
            case Currency.bits:
                color = bitsColor;
                break;
        }

        return color;
    }

    public bool HasEnoughMoney(float amount, Currency currencyType = Currency.money)
    {
        bool hasEnoughMoney = false;
        switch (currencyType)
        {
            case Currency.money:
                if (amount >= money)
                    hasEnoughMoney = true;
                break;
            case Currency.bits:
                if (amount >= bits)
                    hasEnoughMoney = true;
                break;
        }
        return hasEnoughMoney;
    }
}
