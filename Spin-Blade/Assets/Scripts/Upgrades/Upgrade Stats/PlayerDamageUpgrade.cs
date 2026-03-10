using UnityEngine;

public class PlayerDamageUpgrade : MonoBehaviour, IUpgrade
{
	public float damageIncrease;
	public float knockbackIncrease;
	public float knockbackDurationIncrease;

	public float sizeIncrease;

	public void ApplyUpgrade()
	{
		PlayerHealthAndDamage playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

		playerHealth.damage += damageIncrease;
		playerHealth.knockbackDistance += knockbackIncrease;
		playerHealth.knockbackDuration += knockbackDurationIncrease;
		playerHealth.sizeMultiplier += this.sizeIncrease;
	}
}