using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public float money;
    public float moneyMultiplier = 1f;
    public float passiveIncome;
    public GameObject shopMenu;
    public GameObject moneyText;
    public GameObject upgradeParent;
    private List<GameObject> upgrades = new List<GameObject>();
    public AudioClip uiSound;

    [Header("Ui Animations")]
    public AnimationCurve upgradeInfoAnimCurve;

    private void Start()
    {
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
        if (Input.GetKeyDown(key) && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().currentHealth > 0)
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
        if (shopMenu.activeSelf)
        {

            shopMenu.SetActive(!shopMenu.activeSelf);
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
}
