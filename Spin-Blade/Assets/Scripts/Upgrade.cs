using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip buySound;
    public AudioClip errorSound;
    public AudioSource audioSource;

    [Header("Upgrade Values")]
    public Sprite image; // square image, preferibly somethings like 128x128
    public string title;
    public string description;
    public float price;

    public float maxLevel;
    private float currentLevel;

    [Tooltip("This is a multiplier")]
    public float priceIncrease;

    [Header("Upgrade Objects")]
    public GameObject imageObject;
    public GameObject titleObject;
    public GameObject descriptionObject;
    public GameObject priceObject;
    public GameObject maxLevelObject;
    public GameObject buyButton;

    [Header("Effects")]
    float speedIncrease;
    float sizeIncrease;
    float moneyMultiplierIncrease;

    [Header("Assign Objects")]
    public GameObject player;
    public MoneyManager moneyManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        imageObject.GetComponent<Image>().sprite = image;
        titleObject.GetComponent<TextMeshProUGUI>().text = title;
        descriptionObject.GetComponent<TextMeshProUGUI>().text = description;
        priceObject.GetComponent<TextMeshProUGUI>().text = "$" + price.ToString("F1");
        maxLevelObject.GetComponent<TextMeshProUGUI>().text = currentLevel.ToString() + "/" + maxLevel.ToString();

        if (moneyManager.money > price)
            buyButton.GetComponent<Button>().interactable = true;
        else
            buyButton.GetComponent<Button>().interactable = false;
    }

    public void BuyUpgrade()
    {
        if (currentLevel < maxLevel && moneyManager.money >= price)
        {
            moneyManager.money -= price;
            audioSource.PlayOneShot(buySound);
            ApplyEffects();

            // increase price & level
            currentLevel++;
            price *= priceIncrease;
        }
        else
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

    void ApplyEffects()
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        
        // increase speed
        playerMovement.speed += speedIncrease;
        // increase size
        if (sizeIncrease > 0)
            player.transform.localScale = new Vector2(player.transform.localScale.x * sizeIncrease, player.transform.localScale.y * sizeIncrease);
        // increase money multiplier
        moneyManager.moneyMultiplier += moneyMultiplierIncrease;
    }
}
