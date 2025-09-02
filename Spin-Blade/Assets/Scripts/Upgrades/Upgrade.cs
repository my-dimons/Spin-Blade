using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
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
    public MoneyManager.Currency priceCurrencyType = MoneyManager.Currency.money;
    public float priceIncrease;

    [Header("Level")]
    public int maxLevel;
    private int currentLevel;

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
    public Image enemyPopupValueIconMoney;
    public Image enemyPopupValueIconBits;
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

    [Header("-- Extra --")]
    public bool onlyNeedsOnePrecursor; // otherwise needs all precursors to be bought
    public bool precursorsMustBeMaxxed;
    [Header("Buyable Status")]
    public bool canBeBought;
    public bool bought;
    [Header("Locked Status")]
    public bool lockable;
    public bool locked;
    public bool unlockable = true; // can be bought with a special currency when locked
    public MoneyManager.Currency unlockableCurrency = MoneyManager.Currency.bits;
    public float unlockablePrice = 1;
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
        if (!GameObject.FindGameObjectWithTag("MoneyManager"))
        {
            Debug.Log("No money manager found in scene! Please add one.");
            return;
        }

        moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();
        // bg color
        switch (backgroundColorTintDropdown)
        {
            case BackgroundPresetColors.None: backgroundTintColor = Color.white; break;
            case BackgroundPresetColors.Enemy: backgroundTintColor = Utils.ColorFromHex("#FFAEAE"); break;
            case BackgroundPresetColors.Health: backgroundTintColor = Utils.ColorFromHex("#A4FFAC"); break;
            case BackgroundPresetColors.Money: backgroundTintColor = Utils.ColorFromHex("#FFEF99"); break;
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

        UpdateStatText();
        Locking();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (moneyManager == null)
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


    }

    // Update is called once per frame
    void Update()
    {
        if (updateSkillTree)
            UpdateObjects();
    }

    private void UpdateObjects()
    {
        // update background tint
        backgroundObject.GetComponent<Image>().color = backgroundTintColor;

        // disable price when at max lvl (or locked, but not when unlockable)
        if (currentLevel >= maxLevel || locked && !unlockable)
        {
            priceParentObject.SetActive(false);
        }
        else
        {
            priceParentObject.SetActive(true);
        }

        UpdateBuyableStatus();

        UpdateOutlineColor();

        UpdateStatText();

        Locking();

        EnemyPopup(); // (if enemy upg)
    }

    private void UpdateBuyableStatus()
    {
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

        // check if player has enough money, if not disable the button
        Button button = buyButton.GetComponent<Button>();
        if (moneyManager.HasEnoughMoney(price, priceCurrencyType) && canBeBought && currentLevel < maxLevel && !locked)
            button.interactable = true;
        else if (moneyManager.HasEnoughMoney(unlockablePrice, unlockableCurrency) && locked && unlockable)
        {
            button.interactable = true;
            float lockObjOpactiy = 0.5f; // 0-1
            lockObject.GetComponent<Image>().color = new(Color.white.r, Color.white.g, Color.white.b, lockObjOpactiy);
        }
        else
        {
            button.GetComponent<Button>().interactable = false;
            lockObject.GetComponent<Image>().color = Color.white;
        }
    }

    private void EnemyPopup()
    {
        if (enemyPopup)
        {
            enemyPopupObject.SetActive(true);
            UpgradeStats stats = GetComponent<UpgradeStats>();
            Enemy enemy = stats.addEnemy.GetComponent<Enemy>();

            // -- seting stats --

            // set value
            switch (enemy.valueCurrencyType)
            {
                case MoneyManager.Currency.money:
                    enemyPopupValueIconBits.gameObject.SetActive(false);
                    enemyPopupValueIconMoney.gameObject.SetActive(true);
                    break;
                case MoneyManager.Currency.bits:
                    enemyPopupValueIconBits.gameObject.SetActive(true);
                    enemyPopupValueIconMoney.gameObject.SetActive(false);
                    break;
                default:
                    Debug.LogError("Enemy has no currency type set!");
                    break;
            }

            enemyPopupValueText.text = moneyManager.GetMoneyString(enemy.value, enemy.valueCurrencyType).ToString();
            enemyPopupValueText.color = moneyManager.GetCurrencyColor(enemy.valueCurrencyType);

            enemyPopupHealthText.text = enemy.maxHealth.ToString();
            enemyPopupDamageText.text = enemy.damage.ToString();
        }
        else
        {
            enemyPopupObject.SetActive(false);
        }
    }

    private void Locking()
    {
        // get every precursor, then get their postcursors, if *any* postcurosors have been bought, lock this obj
        if (lockable)
        {
            foreach (GameObject precursor in skillTreePrecursors)
            {
                foreach (GameObject postcursor in precursor.GetComponent<Upgrade>().skillTreePostcursors)
                {
                    Upgrade postcursorUpgrade = postcursor.GetComponent<Upgrade>();
                    if (postcursorUpgrade.bought && postcursor != this.gameObject && postcursorUpgrade.lockable)
                    {
                        locked = true;
                        lockable = false;
                    } else if (postcursor.GetComponent<Upgrade>().locked)
                    {
                        lockable = false;
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
        } else
        {
            miniLockObject.SetActive(false);
            lockObject.SetActive(false);
        }
    }

    private void UpdateOutlineColor()
    {
        Image outlineImage = outlineObject.GetComponent<Image>();
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
    }

    private void UpdateStatText()
    {
        // update upgrade name
        name = "Upgrade | " + title.ToLower();

        // update img, and strings
        imageObject.sprite = image;
        titleObject.text = title;
        descriptionObject.text = description;

        // price
        if (locked && unlockable)
        {
            priceObject.text = moneyManager.GetMoneyString(unlockablePrice, unlockableCurrency);
            priceObject.color = moneyManager.GetCurrencyColor(unlockableCurrency);
        }
        else
        {
            priceObject.text = moneyManager.GetMoneyString(price, priceCurrencyType);
            priceObject.color = moneyManager.GetCurrencyColor(priceCurrencyType);
        }

        // max lvl
        maxLevelObject.text = currentLevel.ToString() + "/" + maxLevel.ToString();
    }

    public void BuyUpgrade()
    {
        // double check just in case
        if (!moneyManager.HasEnoughMoney(price, priceCurrencyType) && !locked)
            return;
        else if (!moneyManager.HasEnoughMoney(unlockablePrice, unlockableCurrency) && unlockable && locked)
            return;
        else if (locked && !unlockable)
            return;

        if (unlockable && locked)
        {
            locked = false;
            moneyManager.AddCurrency(-unlockablePrice, unlockableCurrency);
        } else if (!locked)
            moneyManager.AddCurrency(-price, priceCurrencyType);

        Utils.PlayAudioClip(buySound, 0.35f);
        Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(0.1f);
        GetComponent<UpgradeStats>().ApplyEffects();

        if (!bought)
            bought = true;

        // increase price & level
        currentLevel++;
        price *= priceIncrease;
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

            // sfx
            Utils.PlayAudioClip(moneyManager.upgradeHoverSound, 0.15f, 0.07f);
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
