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
	private float boughtUpgradeLevelsPercent = 0;

	private void Start() {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();
	}
	// Update is called once per frame
	void Update() {
		UpdateStatText();
	}

	private void UpdateStatText() {
		double playerRegen = Math.Round(player.maxHeath * (player.regenPerSecond / 100), playerHealthRounding);

		playerRegenText.gameObject.SetActive(playerRegen > 0);

		// Get bought upgrade % if upgrades are found
		if (GetAllUpgrades().Length > 0) {
			boughtUpgradeLevelsPercent = GetPercentOfUnlockedUpgrades();
			SetUpgradePercentTextColor();
		}

		// Set text
		playerHealthText.text = Math.Round(player.currentHealth, playerHealthRounding) + "/" + Math.Round(player.maxHeath, playerHealthRounding);
		playerRegenText.text = "+" + playerRegen + "/s";
		playerDamageText.text = player.damage.ToString();
		unlockedUpgradePercentText.text = "Upgrades: " + Math.Round(boughtUpgradeLevelsPercent * 100, percentTextRounding) + "%";
	}

	/// <summary>
	/// If 100% of upgrades are fully bought, set upgrade % text to all gold, else if >50% are bought, set it to half gold, otherwise set it to white
	/// </summary>
	private void SetUpgradePercentTextColor() {
		if (boughtUpgradeLevelsPercent >= 1) {
			unlockedUpgradePercentText.colorGradient = new VertexGradient(
				MoneyManager.moneyColor,
				MoneyManager.moneyColor,
				MoneyManager.moneyColor,
				MoneyManager.moneyColor);
		} else if (boughtUpgradeLevelsPercent >= 0.5) {
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

	/// <returns>Percentage (0 -> 1) of upgrade levels bought relative to all the upgrade levels</returns>
	private float GetPercentOfUnlockedUpgrades() {
		Upgrade[] buyableUpgrades = FilterBuyableUpgrades(GetAllUpgrades());
		int boughtUpgradeLevels = 0;
		int allUpgradeLevels = 0;
		float percentOfUnlockedUpgradeLevels = 0;

		foreach (Upgrade upg in buyableUpgrades) {
			allUpgradeLevels += upg.maxLevel;
			boughtUpgradeLevels += upg.currentLevel;
		}

		if (allUpgradeLevels != 0) {
			percentOfUnlockedUpgradeLevels = (float)boughtUpgradeLevels / allUpgradeLevels;
		}

		return percentOfUnlockedUpgradeLevels;
	}
}
