using UnityEngine;

public class PlayerWinVisuals : MonoBehaviour
{
    int winFragments = 0;
    int maxFragments;
    public SpriteRenderer playerSpriteRenderer;
    public Sprite[] fragmentSprites;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxFragments = fragmentSprites.Length - 1;
    }

    [ContextMenu("ShowNewFragment")]
    public void ShowNewFragment()
    {
        if (winFragments > maxFragments)
        {
            return;
        } else if (winFragments == maxFragments)
        {
            Debug.Log("MAX FRAGMENTS");
            GetComponent<PlayerMovement>().particleSystemEnum = PlayerMovement.MovementParticleColorEnum.Win;
        }

        playerSpriteRenderer.sprite = fragmentSprites[winFragments];
        winFragments++;
    }
}
