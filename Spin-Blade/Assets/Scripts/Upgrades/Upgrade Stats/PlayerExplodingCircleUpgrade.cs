using UnityEngine;

public class PlayerExplodingCircleUpgrade : MonoBehaviour, IUpgrade
{
    [Header("Stats")]
    public float explodingCircleCooldownIncrease;
    public float explodingCircleDamageMultiplierIncrease;

    [Header("Unlocks")]
    public bool unlockExplodingCircle;
    public bool unlockExplodingCircleKnockback;

    public void ApplyUpgrade()
    {
        PlayerHealthAndDamage playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

        if (!playerHealth.explodingCircle)
            playerHealth.explodingCircle = unlockExplodingCircle;
        if (!playerHealth.explodingCircleKnockback)
            playerHealth.explodingCircleKnockback = unlockExplodingCircleKnockback;

        // stats
        playerHealth.explodingCircleCooldown += explodingCircleCooldownIncrease;
        playerHealth.explodingCircleDamageMultiplier += explodingCircleDamageMultiplierIncrease;
    }
}