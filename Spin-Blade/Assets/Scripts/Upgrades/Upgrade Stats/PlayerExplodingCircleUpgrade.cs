using UnityEngine;

public class PlayerExplodingCircleUpgrade : MonoBehaviour, IUpgrade {
  [Header("Stats")]
  public float explodingCircleCooldownIncrease;

  [Header("Unlocks")]
  public bool unlockExplodingCircle;

  public void ApplyUpgrade() {
    PlayerHealthAndDamage playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

    if (!playerHealth.explodingCircle)
      playerHealth.explodingCircle = unlockExplodingCircle;

    playerHealth.explodingCircleCooldown += explodingCircleCooldownIncrease;
  }
}