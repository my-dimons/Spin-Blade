using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class CurrencyEnemy : MonoBehaviour
{
    public float currencyGain;
    public MoneyManager.Currency currencyType = MoneyManager.Currency.money;

    void AddPlayerMoney()
    {
        MoneyManager moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();

        moneyManager.AddCurrency(currencyGain, currencyType);

        Utils.SpawnFloatingText(GetComponent<Enemy>().deathMoneyText, transform.position, moneyManager.GetMoneyString(moneyManager.CalculateCurrency(currencyGain), currencyType), 6f, 0.3f, 40f, 0.45f, 0.15f, moneyManager.GetCurrencyColor(currencyType));
    }

    private void OnEnable()
    {
        GetComponent<Enemy>().OnDeath += AddPlayerMoney;
    }

    private void OnDisable()
    {
        GetComponent<Enemy>().OnDeath -= AddPlayerMoney;
    }
}
