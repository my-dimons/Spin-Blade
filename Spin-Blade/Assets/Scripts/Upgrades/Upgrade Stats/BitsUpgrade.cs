using UnityEngine;

public class BitsUpgrade : MonoBehaviour, IUpgrade {
	public bool unlockBits;
	public float bitsMultiplierIncrease;
	public float giveBits;

	public void ApplyUpgrade() {
		MoneyManager moneyManager = MoneyManager.Instance;

		if (!moneyManager.bitsUnlocked)
			moneyManager.bitsUnlocked = unlockBits;

		moneyManager.bitsMultiplier += bitsMultiplierIncrease;
		moneyManager.bits += giveBits;
	}
}