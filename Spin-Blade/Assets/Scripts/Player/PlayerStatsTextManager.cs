using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsTextManager : MonoBehaviour {
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
	private float boughtUpgradeTicksPercent = 0;

	private void Start() {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();
	}
	// Update is called once per frame
	void Update() {
		UpdateStatText();
	}

	private void UpdateStatText() {
		double playerRegen = Math.Round(player.maxHeath * (player.regenPerSecond / 100), playerHealthRounding);

		// Set playerRegenText active if player regen is >0
		if (playerRegen > 0) {
			playerRegenText.gameObject.SetActive(true);
		} else {
			playerRegenText.gameObject.SetActive(false);
		}

		// Get bought upgrade % if upgrades are found
		if (GetAllUpgrades().Length > 0) {
			boughtUpgradeTicksPercent = GetPercentOfUnlockedUpgrades();
			SetUpgradePercentTextColor();
		}

		// Set text
		playerHealthText.text = Math.Round(player.currentHealth, playerHealthRounding) + "/" + Math.Round(player.maxHeath, playerHealthRounding);
		playerRegenText.text = "+" + playerRegen + "/s";
		playerDamageText.text = player.damage.ToString();
		unlockedUpgradePercentText.text = "Upgrades: " + Math.Round(boughtUpgradeTicksPercent * 100, percentTextRounding) + "%";
	}

	/// <summary>
	/// If 100% of upgrades are fully bought, set upgrade % text to all gold, else if >50% are bought, set it to half gold, otherwise set it to white
	/// </summary>
	private void SetUpgradePercentTextColor() {
		if (boughtUpgradeTicksPercent >= 1) {
			unlockedUpgradePercentText.colorGradient = new VertexGradient(
				MoneyManager.moneyColor,
				MoneyManager.moneyColor,
				MoneyManager.moneyColor,
				MoneyManager.moneyColor);
		} else if (boughtUpgradeTicksPercent >= 0.5) {
			unlockedUpgradePercentText.colorGradient = new VertexGradient(
				Color.white,
				Color.white,
				MoneyManager.moneyColor,
				MoneyManager.moneyColor);
		} else {
			unlockedUpgradePercentText.color = Color.white;
		}
	}

	private Upgrade[] GetAllUpgrades() {
		GameObject[] upgradeObjects = GameObject.FindGameObjectsWithTag("Upgrade");
		List<Upgrade> upgrades = new List<Upgrade>();

		foreach (GameObject obj in upgradeObjects) {
			if (obj.TryGetComponent<Upgrade>(out Upgrade upg)) {
				upgrades.Add(upg);
			}
		}

		return upgrades.ToArray();
	}

	private Upgrade[] FilterBuyableUpgrades(Upgrade[] upgrades) {
		List<Upgrade> filteredUpgrades = new List<Upgrade>();

		foreach (Upgrade upgrade in upgrades) {
			// Upgrades with a max level of <1 is infinitely buyable and unable to be fully bought
			if (upgrade.maxLevel > 0) {
				filteredUpgrades.Add(upgrade);
			}
		}

		return filteredUpgrades.ToArray();
	}

	private Upgrade[] FilterBoughtUpgrades(Upgrade[] upgrades) {
		List<Upgrade> filteredUpgrades = new List<Upgrade>();

		foreach (Upgrade upgrade in upgrades) {
			if (upgrade.currentLevel > 0) {
				filteredUpgrades.Add(upgrade);
			}
		}

		return filteredUpgrades.ToArray();
	}

	private float GetPercentOfUnlockedUpgrades() {
		Upgrade[] buyableUpgrades = FilterBuyableUpgrades(GetAllUpgrades());
		int boughtUpgradeTicks = 0;
		int allUpgradeTicks = 0;
		float percentOfUnlockedUpgradeTicks = 0;

		foreach (Upgrade upg in buyableUpgrades) {
			allUpgradeTicks += upg.maxLevel;
			boughtUpgradeTicks += upg.currentLevel;
		}

		if (allUpgradeTicks != 0) {
			percentOfUnlockedUpgradeTicks = (float)boughtUpgradeTicks / allUpgradeTicks;
		}

		return percentOfUnlockedUpgradeTicks;
	}
}
