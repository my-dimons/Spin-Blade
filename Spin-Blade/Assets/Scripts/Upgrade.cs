using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip buySound;
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
    public GameObject popupObject;
    public GameObject outlineObject;

    [Header("Effects")]
    float speedIncrease;
    float sizeIncrease;
    float moneyMultiplierIncrease;
    float damageIncrease;
    float regenIncrease;
    float enemySpeedMultiplierIncrease;
    bool unlockHealOnKill;
    float healOnKillAmount;
    // mini saw
    bool spawnMiniSaw;
    float miniSawSpeedIncrease;
    float miniSawDamageIncrease;
    // shooting triangles
    bool unlockShootingTriangles;
    float triangleDamageIncrease;
    float triangleSpeedIncrease;
    float triangleFireRateIncrease;

    [Header("Assign Objects")]
    public GameObject player;
    public MoneyManager moneyManager;
    public EnemyManager enemyManager;

    [Header("Skill Tree")]
    public bool canBeBought;
    public bool bought;
    public GameObject[] skillTreePrecursors; // other skills that need to be bought before this one
    public GameObject[] skillTreeConnectors; // visual connections to other skills
    public Color connectorDisabledColor;
    public Color connectorEnabledColor;

    [Header("Outline Colors")]
    public Color baseOutlineColor;
    public Color boughtOutlineColor;
    public Color fullyBoughtOutlineColor;

    public bool onlyNeedsOnePrecursor; // otherwise needs all precursors to be bought


    bool updateSkillTree = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

        canBeBought = false;
        if (skillTreePrecursors == null)
        {
            canBeBought = true;
        }
        StartCoroutine(SkillTreeDelay());
        IEnumerator SkillTreeDelay()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            updateSkillTree = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (updateSkillTree)
            UpdateSkillTree();
        UpdateStatText();

        if (moneyManager.money > price && canBeBought && currentLevel < maxLevel)
            buyButton.GetComponent<Button>().interactable = true;
        else
            buyButton.GetComponent<Button>().interactable = false;
    }

    private void UpdateSkillTree()
    {
        if (!canBeBought)
        {
            // check if all precursors are bought
            List<GameObject> boughtPrecursors = new List<GameObject>();
            foreach (GameObject precursor in skillTreePrecursors)
            {
                Upgrade precursorUpgrade = precursor.GetComponent<Upgrade>();
                if (precursorUpgrade.bought)
                    boughtPrecursors.Add(precursor);
            }
            if (boughtPrecursors.Count >= skillTreePrecursors.Length || (onlyNeedsOnePrecursor && boughtPrecursors.Count > 0))
                canBeBought = true;
        }

        // set connector color
        foreach (GameObject connector in skillTreeConnectors)
        {
            Image connectorImage = connector.GetComponent<Image>();
            if (canBeBought)
                connectorImage.color = connectorEnabledColor;
            else
                connectorImage.color = connectorDisabledColor;  
        }

        // update outline color
        if (bought)
        {
            if (currentLevel >= maxLevel)
                outlineObject.GetComponent<Image>().color = fullyBoughtOutlineColor;
            else
                outlineObject.GetComponent<Image>().color = boughtOutlineColor;
        }
        else
        {
            outlineObject.GetComponent<Image>().color = baseOutlineColor;
        }
    }

    private void UpdateStatText()
    {
        imageObject.GetComponent<Image>().sprite = image;
        titleObject.GetComponent<TextMeshProUGUI>().text = title;
        descriptionObject.GetComponent<TextMeshProUGUI>().text = description;
        priceObject.GetComponent<TextMeshProUGUI>().text = "$" + price.ToString("F1");
        maxLevelObject.GetComponent<TextMeshProUGUI>().text = currentLevel.ToString() + "/" + maxLevel.ToString();
    }

    public void BuyUpgrade()
    {
        moneyManager.money -= price;
        audioSource.PlayOneShot(buySound);
        ApplyEffects();

        if (!bought)
            bought = true;

        // increase price & level
        currentLevel++;
        price *= priceIncrease;
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
        // increase damage
        playerHealth.damage += damageIncrease;
        // increase regen
        playerHealth.regen += regenIncrease;
        // increase enemy speed multiplier
        enemyManager.enemySpeedMultiplier += enemySpeedMultiplierIncrease;
        // regen on kill
        if (!playerHealth.regenOnKill && unlockHealOnKill)
            playerHealth.regenOnKill = true;
        playerHealth.killRegenAmount += healOnKillAmount;

        // buy mini saws
        if (spawnMiniSaw)
            playerHealth.SpawnSaw();
        // mini saw stats
        playerHealth.miniSawSpeed += miniSawSpeedIncrease;
        foreach (GameObject miniSaw in playerHealth.miniSaws)
        {
            miniSaw.GetComponent<Projectile>().damage += miniSawDamageIncrease;
        }

        // unlock shooting triangles
        if (unlockShootingTriangles)
            playerHealth.unlockedRangedTriangles = true;
        // shooting triangles stats
        playerHealth.triangleDamage += triangleDamageIncrease;
        playerHealth.triangleSpeed += triangleSpeedIncrease;
        playerHealth.triangleFireRate += triangleFireRateIncrease;
    }

    public void TogglePopup(bool enable)
    {
        if (enable)
            gameObject.transform.SetAsLastSibling(); // bring to front
        popupObject.SetActive(enable);
    }
}
