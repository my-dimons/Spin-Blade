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
    public float passiveIncome;
    public GameObject shopMenu;
    public GameObject skillTreeObject;
    public Vector2 shopMenuPos;
    public GameObject moneyText;
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

    private void UpdateMoneyText()
    {
        if (money >= 100)
            moneyText.GetComponent<TextMeshProUGUI>().text = "$" + money.ToString("F0");
        if (money >= 100)
            moneyText.GetComponent<TextMeshProUGUI>().text = "$" + money.ToString("F1");
        else
            moneyText.GetComponent<TextMeshProUGUI>().text = "$" + money.ToString("F2");
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
        money += value * moneyMultiplier;
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
