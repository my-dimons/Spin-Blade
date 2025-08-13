using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip buySound;

    [Space(20)]
    [Header("|--- Upgrade Values ---|")]
    [Header("Details")]
    public Sprite image; // square image, preferibly somethings like 512x512
    public string title;
    [TextArea]
    public string description;

    [Header("Price")]
    public float price;
    public float priceIncrease;

    [Header("Level")]
    public float maxLevel;
    private float currentLevel;

    [Header("|--- Upgrade Values ---|")]
    [Space(20)]
    [Header("|--- Upgrade Objects ---|")]
    [Header("Details Objects")]
    public Image imageObject;
    public TextMeshProUGUI descriptionObject;
    public TextMeshProUGUI titleObject;
    public GameObject priceParentObject;
    public TextMeshProUGUI priceObject;
    public TextMeshProUGUI maxLevelObject;

    [Space(10)]
    [Header("Visual Objects")]
    public GameObject popupObject;
    // enemy popup
    [Space(8)]
    public GameObject enemyPopupObject;
    public TextMeshProUGUI enemyPopupValueText;
    public TextMeshProUGUI enemyPopupDamageText;
    public TextMeshProUGUI enemyPopupHealthText;
    [Space(4)]
    public bool enemyPopup;
    [Space(8)]
    public GameObject outlineObject;
    public GameObject backgroundObject;
    public GameObject tileObject;
    [Space(10)]
    [Header("Lock")]
    public GameObject miniLockObject;
    public GameObject lockObject;
    [Space(10)]
    [Header("Buttons")]
    public GameObject buyButton;
    [Header("|--- Upgrade Objects ---|")]
    [Space(20)]
    [Header("|--- Skill Tree ---|")]

    [Header("Bools")]
    public bool onlyNeedsOnePrecursor; // otherwise needs all precursors to be bought
    public bool precursorsMustBeMaxxed;
    [Space(10)]
    public bool canBeBought;
    public bool bought;
    [Space(10)]
    public bool lockable;
    public bool locked;
    [Space(20)]

    [Header("Precursors/Postcursors")]
    public GameObject[] skillTreePrecursors; // other skills that need to be bought before this one
    public List<GameObject> skillTreePostcursors;

    [Header("-- Colors --")]
    [Header("Outline Colors")]
    public Color baseOutlineColor;
    public Color canBeBoughtOutlineColor;
    public Color boughtOutlineColor;
    public Color fullyBoughtOutlineColor;

    [Header("Background Color")]
    public BackgroundPresetColors backgroundColorTintDropdown;
    public Color backgroundTintColor;
    public enum BackgroundPresetColors
    {
        None,
        Enemy,
        Health,
        Money,
        Damage
    }

    [Space(20)]
    [Header("|--- Skill Tree ---|")]
    [Space(20)]


    bool updateSkillTree = false;

    MoneyManager moneyManager;
    private void OnValidate()
    {
        // bg color
        switch (backgroundColorTintDropdown)
        {
            case BackgroundPresetColors.None: backgroundTintColor = Color.white; break;
            case BackgroundPresetColors.Enemy: backgroundTintColor = Utils.ColorFromHex("#FFAEAE"); break;
            case BackgroundPresetColors.Health: backgroundTintColor = Utils.ColorFromHex("#A4FFAC"); break;
            case BackgroundPresetColors.Money: backgroundTintColor = Utils.ColorFromHex("#FFF4AE"); break;
            case BackgroundPresetColors.Damage: backgroundTintColor = Utils.ColorFromHex("#AED4FF"); break;
        }

        // enemy popup
        if (GetComponent<UpgradeStats>().addEnemy != null)
        {
            enemyPopup = true;
        }
        else
        {
            enemyPopup = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();


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
            UpdateObjects();

        Button button = buyButton.GetComponent<Button>();
        if (moneyManager.money >= price && canBeBought && currentLevel < maxLevel && !locked)
            button.interactable = true;
        else
            button.GetComponent<Button>().interactable = false;
    }

    private void UpdateObjects()
    {
        // update background tint
        backgroundObject.GetComponent<Image>().color = backgroundTintColor;

        if (!canBeBought)
        {
            // check if all precursors are bought
            List<GameObject> boughtPrecursors = new();
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


        Image outlineImage = outlineObject.GetComponent<Image>();
        // update outline color
        if ((canBeBought || bought) && !locked)
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

        // locking

        // get every precursor, then get their postcursors, if *any* postcurosrs have been bought, lock this obj
        if (lockable)
        {
            foreach(GameObject precursor in skillTreePrecursors)
            {
                foreach (GameObject postcursor in precursor.GetComponent<Upgrade>().skillTreePostcursors)
                {
                    if (postcursor.GetComponent<Upgrade>().bought && postcursor != this.gameObject)
                    {
                        locked = true;
                    }
                }
            }
        }

        // locking visual objects
        if (lockable && !locked)
        {
            miniLockObject.SetActive(true);
            lockObject.SetActive(false);
        }

        else if (locked)
        {
            miniLockObject.SetActive(false);
            lockObject.SetActive(true);
        }

        // disable price when at max lvl (or locked)
        if (currentLevel >= maxLevel || locked)
        {
            priceParentObject.SetActive(false);
        }
        else
        {
            priceParentObject.SetActive(true);
        }

        // update enemy text
        if (enemyPopup)
        {
            enemyPopupObject.SetActive(true);
            UpgradeStats stats = GetComponent<UpgradeStats>();
            enemyPopupValueText.text = stats.addEnemy.GetComponent<Enemy>().value.ToString();
            enemyPopupHealthText.text = stats.addEnemy.GetComponent<Enemy>().maxHealth.ToString();
            enemyPopupDamageText.text = stats.addEnemy.GetComponent<Enemy>().damage.ToString();
        } else
        {
            enemyPopupObject.SetActive(false);
        }
    }

    private void UpdateStatText()
    {
        imageObject.sprite = image;
        titleObject.text = title;
        descriptionObject.text = description;

        if (price >= 1000)
            priceObject.text = "$" + price.ToString("F0");
        else if (price >= 100)
            priceObject.text = "$" + price.ToString("F1");
        else 
            priceObject.text = "$" + price.ToString("F2");

        maxLevelObject.text = currentLevel.ToString() + "/" + maxLevel.ToString();
    }

    public void BuyUpgrade()
    {
        moneyManager.money -= price;
        Utils.PlayClip(buySound, 0.35f);
        Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(0.1f);
        GetComponent<UpgradeStats>().ApplyEffects();

        if (!bought)
            bought = true;

        // increase price & level
        currentLevel++;
        price *= priceIncrease;
        UpdateStatText();
    }

    public void TogglePopup(bool enable)
    {
        float animationSpeed = 0.1f;

        if (enable)
        {
            gameObject.transform.SetAsLastSibling(); // bring to front
            
            popupObject.SetActive(enable);
            // enable popup
            StartCoroutine(Utils.AnimateValue(0f, .7f, animationSpeed, moneyManager.upgradeInfoAnimCurve,
                value => popupObject.transform.localScale = Vector3.one * value, useRealtime: true));

            // upgrade tile object (make bigger)
            StartCoroutine(Utils.AnimateValue(1f, 1.3f, animationSpeed, moneyManager.upgradeInfoAnimCurve,
                value => tileObject.transform.localScale = Vector3.one * value, useRealtime: true));
        }
        else
        {
            // disable popup
            StartCoroutine(Utils.AnimateValue(.7f, 0f, animationSpeed, moneyManager.upgradeInfoAnimCurve,
                 value => popupObject.transform.localScale = Vector3.one * value, useRealtime: true));
            StartCoroutine(Utils.EnableObjectDelay(popupObject, enable, animationSpeed));

            // object (make smaller)
            StartCoroutine(Utils.AnimateValue(1.3f, 1f, animationSpeed, moneyManager.upgradeInfoAnimCurve,
                value => tileObject.transform.localScale = Vector3.one * value, useRealtime: true));
        }
    }
}
