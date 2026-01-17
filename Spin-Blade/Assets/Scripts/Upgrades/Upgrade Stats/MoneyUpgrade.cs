using UnityEngine;

public class MoneyUpgrade : MonoBehaviour, IUpgrade
{
    public float moneyMultiplierIncrease;
    public float passiveIncomeIncrease;

    public void ApplyUpgrade()
    {
        MoneyManager moneyManager = MoneyManager.Instance;

        moneyManager.moneyMultiplier += moneyMultiplierIncrease;
        moneyManager.passiveIncome += passiveIncomeIncrease;
    }
}