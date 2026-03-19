using UnityEngine;

public class PlayerMiniSawsUpgrade : MonoBehaviour, IUpgrade {
  [Header("Unlocks")]
  public bool spawnMiniSaw;

  [Header("Stats")]
  public float miniSawSpeedIncrease;
  public float miniSawDamageIncrease;
  public void ApplyUpgrade() {
    PlayerHealthAndDamage playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

    if (spawnMiniSaw)
      playerHealth.SpawnSaw();

    playerHealth.miniSawSpeed += miniSawSpeedIncrease;
    playerHealth.miniSawDamage += miniSawDamageIncrease;
  }
}