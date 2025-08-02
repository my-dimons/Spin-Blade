using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    public GameObject backgroundObject;
    public GameObject tileObject;

    [Header("Effects")]
    public float speedIncrease;
    public float sizeIncrease;
    public float healthIncrease;
    public float moneyMultiplierIncrease;
    public float passiveIncomeIncrease;
    public float damageIncrease;
    public float regenIncrease;
    public float knockbackIncrease;
    public float stunDurationIncrease;
    public bool unlockHealOnKill;
    public float healOnKillAmount;
    // mini saw
    public bool spawnMiniSaw;
    public float miniSawSpeedIncrease;
    public float miniSawDamageIncrease;
    // shooting triangles
    public bool unlockShootingTriangles;
    public bool rangedAutofire;
    public float triangleDamageIncrease;
    public float triangleSpeedIncrease;
    public float triangleFireRateIncrease;
    // enemies
    public float enemySpeedMultiplierIncrease;
    public float enemyDifficultyIncrease;
    public float enemySpawnRateIncrease;
    public GameObject addEnemy;

    [Header("Assign Objects")]
    public GameObject player;
    public MoneyManager moneyManager;
    public EnemyManager enemyManager;

    [Header("Skill Tree")]
    public bool canBeBought;
    public bool bought; 
    public bool onlyNeedsOnePrecursor; // otherwise needs all precursors to be bought
    public bool precursorsMustBeMaxxed;

    public GameObject[] skillTreePrecursors; // other skills that need to be bought before this one
    public GameObject[] skillTreeConnectors; // visual connections to other skills
    public Color connectorDisabledColor;
    public Color connectorDisabledColorMaxxed;
    public Color connectorEnabledColor;

    [Header("Outline Colors")]
    public Color baseOutlineColor;
    public Color canBeBoughtOutlineColor;
    public Color boughtOutlineColor;
    public Color fullyBoughtOutlineColor;

    [Header("Background Color")]
    public Color backgroundTintColor;

    bool updateSkillTree = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

        backgroundObject.GetComponent<Image>().color = backgroundTintColor;

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

        UpdateStatText();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateSkillTree)
            UpdateSkillTree();

        Button button = buyButton.GetComponent<Button>();
        if (moneyManager.money >= price && canBeBought && currentLevel < maxLevel)
            button.interactable = true;
        else
            button.GetComponent<Button>().interactable = false;
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
            {
                if (precursorsMustBeMaxxed)
                {
                    // check if all precursors are maxed
                    bool allMaxed = true;
                    foreach (GameObject precursor in skillTreePrecursors)
                    {
                        Upgrade precursorUpgrade = precursor.GetComponent<Upgrade>();
                        if (precursorUpgrade.currentLevel < precursorUpgrade.maxLevel)
                        {
                            allMaxed = false;
                            break;
                        }
                    }
                    canBeBought = allMaxed;
                }
                else
                    canBeBought = true;
            }
        }

        // set connector color
        foreach (GameObject connector in skillTreeConnectors)
        {
            Image connectorImage = connector.GetComponent<Image>();
            if (canBeBought)
                connectorImage.color = connectorEnabledColor;
            else if (!canBeBought && precursorsMustBeMaxxed)
                connectorImage.color = connectorDisabledColorMaxxed;
            else
                connectorImage.color = connectorDisabledColor;  
        }

        Image outlineImage = outlineObject.GetComponent<Image>();
        // update outline color
        if (canBeBought || bought)
        {
            if (currentLevel >= maxLevel)
                outlineImage.color = fullyBoughtOutlineColor;
            else if (currentLevel > 0)
                outlineImage.color = boughtOutlineColor;
            else 
                outlineImage.color = canBeBoughtOutlineColor;
        }
        else
        {
            outlineImage.color = baseOutlineColor;
        }
    }

    private void UpdateStatText()
    {
        imageObject.GetComponent<Image>().sprite = image;
        titleObject.GetComponent<TextMeshProUGUI>().text = title;
        descriptionObject.GetComponent<TextMeshProUGUI>().text = description;

        if (price >= 1000)
            priceObject.GetComponent<TextMeshProUGUI>().text = "$" + price.ToString("F0");
        else if (price >= 100)
            priceObject.GetComponent<TextMeshProUGUI>().text = "$" + price.ToString("F1");
        else 
            priceObject.GetComponent<TextMeshProUGUI>().text = "$" + price.ToString("F2");

        maxLevelObject.GetComponent<TextMeshProUGUI>().text = currentLevel.ToString() + "/" + maxLevel.ToString();
    }

    public void BuyUpgrade()
    {
        moneyManager.money -= price;
        audioSource.PlayOneShot(buySound, 0.4f);
        Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(0.1f);
        ApplyEffects();

        if (!bought)
            bought = true;

        // increase price & level
        currentLevel++;
        price *= priceIncrease;
        UpdateStatText();
    }

    void ApplyEffects()
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        // increase speed
        playerMovement.speed += speedIncrease;
        // increase size
        playerHealth.sizeMultiplier += sizeIncrease;
        // increase money multiplier
        moneyManager.moneyMultiplier += moneyMultiplierIncrease;
        // increase passive income
        moneyManager.passiveIncome += passiveIncomeIncrease;
        // increase max health
        playerHealth.IncreaseMaxHealth(healthIncrease);
        // increase damage
        playerHealth.damage += damageIncrease;
        // increase regen
        playerHealth.regen += regenIncrease;
        // increase knockback & stun time
        playerHealth.knockbackForce += knockbackIncrease;
        playerHealth.stunDuration += stunDurationIncrease;
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
        if (!playerHealth.autofireTriangles)
            playerHealth.autofireTriangles = rangedAutofire;

        // add enemy
        if (addEnemy != null)
        {
            EnemyManager enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
            enemyManager.enemies.Add(addEnemy);
        }

        enemyManager.spawnRate += enemySpawnRateIncrease;
    }

    public void TogglePopup(bool enable)
    {
        float duration = 0.1f;
        if (enable)
        {
            popupObject.SetActive(enable);
            gameObject.transform.SetAsLastSibling(); // bring to front
            // popup
            StartCoroutine(Utils.AnimateValue(0f, .7f, duration, moneyManager.upgradeInfoAnimCurve,
                value => popupObject.transform.localScale = Vector3.one * value, useRealtime: true));
            // object (make bigger)
            StartCoroutine(Utils.AnimateValue(1f, 1.3f, duration, moneyManager.upgradeInfoAnimCurve,
                value => tileObject.transform.localScale = Vector3.one * value, useRealtime: true));
        }
        else
        {
            // popup
            StartCoroutine(Utils.AnimateValue(.7f, 0f, duration, moneyManager.upgradeInfoAnimCurve,
                 value => popupObject.transform.localScale = Vector3.one * value, useRealtime: true));
            // object (make smaller)
            StartCoroutine(Utils.AnimateValue(1.3f, 1f, duration, moneyManager.upgradeInfoAnimCurve,
                value => tileObject.transform.localScale = Vector3.one * value, useRealtime: true));
            StartCoroutine(Utils.EnableObjectDelay(popupObject, enable, duration));
        }
    }


}
