using UnityEngine;

public class PlayerMiniSawsUpgrade : MonoBehaviour, IUpgrade
{
	[Header("Unlocks")]
	public bool spawnMiniSaw;

	[Header("Stats")]
	public float miniSawSpeedIncrease;
	public float miniSawDamageIncrease;
	public void ApplyUpgrade()
	{
		PlayerHealthAndDamage playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

		if (spawnMiniSaw)
			playerHealth.SpawnSaw();

		foreach (GameObject miniSaw in playerHealth.miniSaws)
		{
			miniSaw.GetComponent<PlayerMiniSaw>().IncreaseSpeed(miniSawSpeedIncrease);
			miniSaw.GetComponent<Projectile>().damage += miniSawDamageIncrease;
		}
	}
}