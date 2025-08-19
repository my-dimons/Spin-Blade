using UnityEngine;

public class BomberEnemy : MonoBehaviour
{
    public float explosionRadius = 6;
    private bool hasExploded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Enemy>().OnDeath += OnDeath;
    }

    private void OnDestroy()
    {
        GetComponent<Enemy>().OnDeath -= OnDeath;
    }
    void OnDeath()
    {
        if (hasExploded) return; // prevent multiple explosions
        hasExploded = true;

        PlayerHealthAndDamage playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

        Debug.Log("Bomber enemy died");
        if (playerHealth != null) 
            playerHealth.ExplodeCircle(transform.position, playerHealth.damage, explosionRadius, true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
