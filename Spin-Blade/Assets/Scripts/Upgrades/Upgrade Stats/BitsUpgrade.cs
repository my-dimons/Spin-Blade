using UnityEngine;

public class BitsUpgrade : MonoBehaviour, IUpgrade {
  public bool unlockBits;
  public float bitsMultiplierIncrease;
  [Space(8)]
  public float giveBits;
  public bool updateBitsText;

  private void Update() {
    if (TryGetComponent(out Upgrade upgrade) && updateBitsText) {
      upgrade.description = $"gives +{MoneyManager.Instance.CalculateCurrency(giveBits, MoneyManager.Currency.bits)} bits";
    }
  }
  public void ApplyUpgrade() {
    MoneyManager moneyManager = MoneyManager.Instance;

    if (!moneyManager.bitsUnlocked)
      moneyManager.bitsUnlocked = unlockBits;

    moneyManager.bitsMultiplier += bitsMultiplierIncrease;
    moneyManager.bits += MoneyManager.Instance.CalculateCurrency(giveBits, MoneyManager.Currency.bits);
  }
}