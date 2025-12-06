using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class WinVisuals : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Image imageRenderer;
    public Sprite[] fragmentSprites = new Sprite[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        imageRenderer = GetComponent<Image>();
    }

    private void Update()
    {
        ShowFragments();
    }

    private void ShowFragments()
    {
        Sprite winSprite = fragmentSprites[WinManager.winFragements];

        if (spriteRenderer != null && spriteRenderer.sprite != winSprite)
            spriteRenderer.sprite = winSprite;
        
        if (imageRenderer != null && imageRenderer.sprite != winSprite)
            imageRenderer.sprite = winSprite;
    }
}
