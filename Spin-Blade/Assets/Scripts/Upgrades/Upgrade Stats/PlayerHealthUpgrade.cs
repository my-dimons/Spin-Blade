using UnityEngine;

public class PlayerHealthUpgrade : MonoBehaviour, IUpgrade
{
	public float healthIncrease;
	public float regenIncrease;
	public float healthOnKillIncrease;
	public int reviveIncreases;

	public void ApplyUpgrade()
	{
		PlayerHealthAndDamage playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

		playerHealth.IncreaseMaxHealth(healthIncrease);
		playerHealth.revives += reviveIncreases;
		playerHealth.regenPerSecond += regenIncrease;
		playerHealth.killRegenAmount += healthOnKillIncrease;
	}
}