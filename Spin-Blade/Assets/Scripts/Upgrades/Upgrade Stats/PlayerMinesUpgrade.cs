using UnityEngine;

public class PlayerMinesUpgrade : MonoBehaviour, IUpgrade
{
	[Header("Stats")]
	public float mineExplosionRadiusIncrease;
	public float mineDamageMultiplierIncrease;
	public float mineKnockbackIncrease;
	public float mineLifetimeIncrease;
	public float mineCooldownIncrease;

	[Header("Unlocks")]
	public bool unlockMines;
	public bool explodingMines;

	public void ApplyUpgrade()
	{
		PlayerHealthAndDamage playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

		if (!playerHealth.mines)
			playerHealth.mines = unlockMines;
		if (!playerHealth.explodingMines)
			playerHealth.explodingMines = explodingMines;

		// stats
		playerHealth.mineExplosionRadius += mineExplosionRadiusIncrease;
		playerHealth.mineDamageMultiplier += mineDamageMultiplierIncrease;
		playerHealth.mineDamageMultiplier += mineKnockbackIncrease;
		playerHealth.minesLifetime += mineLifetimeIncrease;
		playerHealth.minesCooldown += mineCooldownIncrease;
	}
}
