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
    public GameObject imageObject;
    public GameObject titleObject;
    public GameObject descriptionObject;
    public GameObject priceObject;
    public GameObject maxLevelObject;
    [Header("Visual Objects")]
    public GameObject popupObject;
    public GameObject outlineObject;
    public GameObject backgroundObject;
    public GameObject tileObject;
    [Header("Buttons")]
    public GameObject buyButton;
    [Header("|--- Upgrade Objects ---|")]
    [Space(20)]
    [Header("|--- Skill Tree ---|")]

    public bool onlyNeedsOnePrecursor; // otherwise needs all precursors to be bought
    public bool precursorsMustBeMaxxed;

    public GameObject[] skillTreePrecursors; // other skills that need to be bought before this one

    [Header("|- Colors -|")]
    [Header("Outline Colors")]
    public Color baseOutlineColor;
    public Color canBeBoughtOutlineColor;
    public Color boughtOutlineColor;
    public Color fullyBoughtOutlineColor;

    [Header("Background Color")]
    public Color backgroundTintColor;
    [Header("|- Colors -|")]
    [Space(20)]
    [Header("|--- Skill Tree ---|")]
    [Space(20)]
    public bool canBeBought;
    public bool bought;

    bool updateSkillTree = false;

    MoneyManager moneyManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();

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
