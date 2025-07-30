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

    [Header("Effects")]
    float speedIncrease;

    [Header("Assign Objects")]
    public GameObject player;
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
    }

    public void BuyUpgrade()
    {
        MoneyManager moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();
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

        playerMovement.speed += speedIncrease;
    }
}
