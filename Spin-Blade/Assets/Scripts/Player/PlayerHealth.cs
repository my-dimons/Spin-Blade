using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHeath = 2;
    public float currentHealth;
    public float regen;

    public bool regenOnKill; // regen health when killing an enemy
    public float killRegenAmount = 0f; // amount of health to regen on kill
    [Header("Health Display")]
    public Image healthBar;
    public GameObject deathScreen;

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
    public float miniSawSpeed = 1f;
    public float hexagonDamage = 1f;

    [Header("Ranged Triangles")]
    public bool unlockedRangedTriangles; // triangles that shoot at enemies
    public GameObject rotationPivot; // used to properly rotate the triangles
    public GameObject trianglePrefab;
    public float triangleDamage = 1f;
    public float triangleSpeed = 1f;
    public float triangleFireRate = 0.8f; // seconds between shots
    public bool autofireTriangles;
    float triangleFireTimer = 0f;

    //[Header("Taking Damage")]


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHeath;
        baseSize = transform.localScale;
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

        currentHealth += regen * Time.deltaTime;
        Mathf.Clamp(currentHealth, 0, maxHeath);
        healthBar.fillAmount = (float)currentHealth / maxHeath;
        if (currentHealth <= 0)
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
        GameObject triangle = Instantiate(trianglePrefab, transform.position, Quaternion.identity);
        triangle.GetComponent<Projectile>().damage = triangleDamage;
        triangle.GetComponent<Projectile>().speed = triangleSpeed;

        // 2. Calculate direction AWAY from center
        Vector2 direction = (transform.position - rotationPivot.transform.position).normalized;

        // 3. Initialize projectile
        triangle.GetComponent<TriangleProjectile>().Initialize(direction);

    }

    void Death()
    {
        Time.timeScale = 0;
        deathScreen.SetActive(true);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Mathf.Clamp(currentHealth, 0, maxHeath);
    }
    public void Heal(float heal)
    {
        currentHealth -= heal;
        Mathf.Clamp(currentHealth, 0, maxHeath);
    }
    public void IncreaseMaxHealth(float amount)
    {
        maxHeath += amount;
        currentHealth += amount;
        Mathf.Clamp(currentHealth, 0, maxHeath);
    }
}
