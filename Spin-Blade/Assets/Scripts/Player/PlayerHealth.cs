using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHeath;
    public float currentHealth;
    public int revives;
    public float regenPerSecond;
    public float killRegenAmount = 0f; // amount of health to regen on kill

    [Header("Health Display")]
    public AudioClip fullHealthSound;
    public Image healthBar;
    public GameObject deathScreen;
    public DamageFlash circleDamageFlash;
    public Color circleDamageFlashColor;
    public Color circleMoneyGainHitFlashColor;
    public Color circleFullHealFlashColor;

    bool dead;
    private float oldHealth;
    private float oldMaxHealth;

    [Header("Dealing Damage")]
    private Vector2 baseSize;
    public float sizeMultiplier = 1f;
    public float knockbackForce = 10f;
    public float stunDuration = 1f;
    public float damage = 1;

    public bool explodingCircle; // the circle explodes occationally

    [Header("Mini Saws")]
    public List<GameObject> miniSaws;
    public GameObject miniSawPrefab;
    public GameObject[] miniSawWaypoints;
    public float miniSawBaseSpeed = 1f;
    public float hexagonDamage = 1f;

    [Header("Ranged Triangles")]
    public bool unlockedRangedTriangles; // triangles that shoot at enemies
    public GameObject rotationPivot; // used to properly rotate the triangles
    public GameObject trianglePrefab;
    public float triangleDamage = 1f;
    public float triangleSpeed = 1f;
    public float triangleFireRate = 0.8f; // seconds between shots
    public bool autofireTriangles;
    public bool homingTriangles;
    public bool piercingTriangles;
    float triangleFireTimer = 0f;

    [Header("Audio")]
    public AudioClip deathSound;
    public AudioClip shootSound;
    public AudioClip hitSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHeath;
        baseSize = transform.localScale;

        if (GameObject.FindGameObjectWithTag("PVars").GetComponent<PersistentVariables>().infiniteMode)
        {
            regenPerSecond = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = baseSize * sizeMultiplier;

        triangleFireTimer += Time.deltaTime;

        if ((Input.GetKeyDown(KeyCode.Space) || autofireTriangles) && triangleFireTimer >= triangleFireRate && unlockedRangedTriangles)
        {
            triangleFireTimer = 0f;
            SpawnTriangle();
        }


        float regenAmount = currentHealth * regenPerSecond / 100;
        currentHealth = Mathf.Clamp(currentHealth += regenAmount * Time.deltaTime, 0, maxHeath);

        // full health ping
        if (currentHealth >= maxHeath && oldHealth < maxHeath && oldHealth != oldMaxHealth)
        {
            Utils.PlayClip(fullHealthSound, 0.7f);
            circleDamageFlash.Flash(circleFullHealFlashColor);
        }

        oldHealth = currentHealth;
        oldMaxHealth = maxHeath;


        healthBar.fillAmount = (float)currentHealth / maxHeath;
        if (currentHealth <= 0 && !dead)
        {
            Death();
        }
    }
    public void SpawnSaw()
    {
        GameObject randomSaw = miniSawWaypoints[Random.Range(0, miniSawWaypoints.Length)];
        GameObject saw = Instantiate(miniSawPrefab, randomSaw.transform.position, Quaternion.identity);
        miniSaws.Add(saw);
        saw.GetComponent<PlayerMiniSaw>().waypoints = new List<GameObject>(miniSawWaypoints);
        saw.GetComponent<PlayerMiniSaw>().currentWaypoint = randomSaw;
    }

    public void SpawnTriangle()
    {
        Utils.PlayClip(shootSound, 0.15f);
        GameObject triangle = Instantiate(trianglePrefab, transform.position, Quaternion.identity);
        triangle.GetComponent<Projectile>().damage = triangleDamage;
        triangle.GetComponent<Projectile>().speed = triangleSpeed;

        // Calculate direction AWAY from center
        Vector2 direction = (transform.position - rotationPivot.transform.position).normalized;

        triangle.GetComponent<TriangleProjectile>().Initialize(direction, homingTriangles, piercingTriangles);

    }

    void Death()
    {
        // revive and kill all enemies
        if (revives > 0)
        {
            currentHealth = maxHeath;
            revives--;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<Enemy>().Death(false);
            }

            return;
        }

        dead = true;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().tutorialText.gameObject.SetActive(false); 
        Utils.PlayClip(deathSound, 1.3f);
        Time.timeScale = 0;
        deathScreen.SetActive(true);
    }

    public void TakeDamage(float damage, bool flashMoney = false)
    {
        Utils.PlayClip(hitSound, 1f);
        currentHealth -= damage;

        if (currentHealth > 0 && !flashMoney)
            circleDamageFlash.Flash(circleDamageFlashColor);
        else if (currentHealth > 0 && flashMoney)
            circleDamageFlash.Flash(circleMoneyGainHitFlashColor);

        Mathf.Clamp(currentHealth, 0, maxHeath);
    }
    public void Heal(float heal)
    {
        currentHealth += heal;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHeath);
    }
    public void IncreaseMaxHealth(float amount)
    {
        maxHeath += amount;
        currentHealth += amount;
        Mathf.Clamp(currentHealth, 0, maxHeath);
    }
}
