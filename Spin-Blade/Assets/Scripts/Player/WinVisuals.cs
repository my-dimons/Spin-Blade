using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WinVisuals : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] fragmentSprites = new Sprite[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ShowFragments();
    }

    private void ShowFragments()
    {
        if (spriteRenderer.sprite != fragmentSprites[WinManager.winFragements])
            spriteRenderer.sprite = fragmentSprites[WinManager.winFragements];
    }
}
