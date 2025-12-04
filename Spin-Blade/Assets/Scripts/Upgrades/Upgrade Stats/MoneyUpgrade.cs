using UnityEngine;

public class MoneyUpgrade : MonoBehaviour, IUpgrade
{
    public float moneyMultiplierIncrease;
    public float passiveIncomeIncrease;

    public void ApplyUpgrade()
    {
        MoneyManager moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();

        moneyManager.moneyMultiplier += moneyMultiplierIncrease;
        moneyManager.passiveIncome += passiveIncomeIncrease;
    }
}