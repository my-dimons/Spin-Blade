using UnityEngine;

public class PlayerTriangleProjectilesUpgrade : MonoBehaviour, IUpgrade
{
	[Header("Unlocks")]
	public bool unlockShootingTriangles;
	public bool rangedAutofire;
	public bool homingTriangles;
	public bool piercingTriangles;

	[Header("Stats")]
	public float triangleDamageIncrease;
	public float triangleSpeedIncrease;
	public float triangleFireRateIncrease;

	public void ApplyUpgrade()
	{
		PlayerHealthAndDamage playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

		if (unlockShootingTriangles)
			playerHealth.unlockedRangedTriangles = true;

		if (!playerHealth.autofireTriangles)
			playerHealth.autofireTriangles = rangedAutofire;
		if (!playerHealth.homingTriangles)
			playerHealth.homingTriangles = homingTriangles;
		if (!playerHealth.piercingTriangles)
			playerHealth.piercingTriangles = piercingTriangles;

		playerHealth.triangleDamage += triangleDamageIncrease;
		playerHealth.triangleSpeed += triangleSpeedIncrease;
		playerHealth.triangleFireRate += triangleFireRateIncrease;
	}
}