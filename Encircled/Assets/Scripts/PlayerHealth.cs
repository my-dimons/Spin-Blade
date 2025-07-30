using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHeath = 2;
    public int currentHealth;
    [Header("Health Display")]
    public Image healthBar;
    public GameObject deathScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHeath;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = (float)currentHealth / maxHeath;
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Time.timeScale = 0;
        deathScreen.SetActive(true);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Mathf.Clamp(currentHealth, 0, maxHeath);
    }
    public void Heal(int heal)
    {
        currentHealth -= heal;
        Mathf.Clamp(currentHealth, 0, maxHeath);
    }
    public void ChangeMaxHealth(int amount)
    {
        maxHeath += amount;
        currentHealth += amount;
        Mathf.Clamp(currentHealth, 0, maxHeath);
    }
}
