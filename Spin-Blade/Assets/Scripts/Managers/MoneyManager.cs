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
            Time.timeScale = Time.timeScale == 0 ? 1 : 0; // pause or unpause the game
            shopMenu.SetActive(!shopMenu.activeSelf);
            
            foreach (GameObject upgrade in upgrades)
            {
                foreach (Transform child in upgrade.transform)
                {
                    if (child.CompareTag("UpgradeDsc"))
                        child.gameObject.SetActive(false);
                }
            }
        }

        // update money text
        moneyText.GetComponent<TextMeshProUGUI>().text = "$" + money.ToString("F1");
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
