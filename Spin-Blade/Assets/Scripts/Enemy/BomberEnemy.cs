using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class BomberEnemy : MonoBehaviour {
	[Header("Bomber Enemy")]
	public float explosionRadius = 6;
	private bool hasExploded = false;

	void BomberDeath() {
		if (hasExploded) return; // prevent multiple explosions
		hasExploded = true;

		PlayerHealthAndDamage playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

		Debug.Log("Bomber enemy died");
		if (playerHealth != null)
			playerHealth.ExplodeCircle(transform.position, playerHealth.damage, explosionRadius, true);
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, explosionRadius);
	}

	private void OnEnable() {
		GetComponent<Enemy>().OnDeath += BomberDeath;
	}

	private void OnDisable() {
		GetComponent<Enemy>().OnDeath -= BomberDeath;
	}
}
