using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsTextManager : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text playerHealthText;
    public TMP_Text playerRegenText;
    public TMP_Text playerDamageText;

    [Space(5)]

    public TMP_Text unlockedUpgradePercentText;

    [Header("Constants")]
    [SerializeField] private int percentTextRounding = 2;
    [SerializeField] private int playerHealthRounding = 2;

    private PlayerHealthAndDamage player;
    private float boughtUpgradePercent = 0;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateStatText();
    }

    private void UpdateStatText()
    {
        if (GetAllUpgrades().Length > 0)
        {
            boughtUpgradePercent = GetPercentOfUnlockedUpgrades();
        }

        playerHealthText.text = Math.Round(player.currentHealth, playerHealthRounding) + "/" + Math.Round(player.maxHeath, playerHealthRounding);
        playerRegenText.text = "+" + Math.Round(player.regenPerSecond, playerHealthRounding) + "/s";
        playerDamageText.text = player.damage.ToString();
        unlockedUpgradePercentText.text = "Upgrades: " + Math.Round(boughtUpgradePercent * 100, percentTextRounding) + "%";
    }

    private Upgrade[] GetAllUpgrades()
    {
        GameObject[] upgradeObjects = GameObject.FindGameObjectsWithTag("Upgrade");
        List<Upgrade> upgrades = new List<Upgrade>();

        foreach (GameObject obj in upgradeObjects)
        {
            if (obj.TryGetComponent<Upgrade>(out Upgrade upg)) 
            {
                upgrades.Add(upg);
            }
        }

        return upgrades.ToArray();
    }

    private Upgrade[] FilterBuyableUpgrades(Upgrade[] upgrades)
    {
        List<Upgrade> filteredUpgrades = new List<Upgrade>();

        foreach (Upgrade upgrade in upgrades)
        {
            // Upgrades with a max level of <1 is infinitly buyable and unable to be fully bought
            if (upgrade.maxLevel > 0)
            {
                filteredUpgrades.Add(upgrade);
            }
        }

        return filteredUpgrades.ToArray();
    }

    private Upgrade[] FilterBoughtUpgrades(Upgrade[] upgrades)
    {
        List<Upgrade> filteredUpgrades = new List<Upgrade>();

        foreach (Upgrade upgrade in upgrades)
        {
            if (upgrade.currentLevel > 0)
            {
                filteredUpgrades.Add(upgrade);
            }
        }

        return filteredUpgrades.ToArray();
    }

    private float GetPercentOfUnlockedUpgrades()
    {
        Upgrade[] buyableUpgrades = FilterBuyableUpgrades(GetAllUpgrades());

        float allUpgradesNumber = buyableUpgrades.Length;
        float boughtUpgradesNumber = FilterBoughtUpgrades(buyableUpgrades).Length;

        float percentOfUnlockedUpgrades = 0;

        if (allUpgradesNumber != 0)
        {
            percentOfUnlockedUpgrades = boughtUpgradesNumber / allUpgradesNumber;
        }

        return percentOfUnlockedUpgrades;
    }
}
