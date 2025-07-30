using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public float money;
    public GameObject shopMenu;
    public GameObject moneyText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // toggle shop
        KeyCode key = KeyCode.Tab;
        if (Input.GetKeyDown(key))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0; // pause or unpause the game
            shopMenu.SetActive(!shopMenu.activeSelf);
        }

        // update money text
        moneyText.GetComponent<TextMeshProUGUI>().text = "$" + money.ToString("F1");
    }
}
