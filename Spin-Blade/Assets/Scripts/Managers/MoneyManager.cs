using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public float money;
    public float moneyMultiplier = 1f;
    public GameObject shopMenu;
    public GameObject moneyText;

    // Update is called once per frame
    void Update()
    {
        // toggle shop
        KeyCode key = KeyCode.Tab;
        if (Input.GetKeyDown(key) && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().currentHealth > 0)
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0; // pause or unpause the game
            shopMenu.SetActive(!shopMenu.activeSelf);
        }

        // update money text
        moneyText.GetComponent<TextMeshProUGUI>().text = "$" + money.ToString("F1");
    }

    public void AddMoney(float value)
    {
        money += value * moneyMultiplier;
    }
}
