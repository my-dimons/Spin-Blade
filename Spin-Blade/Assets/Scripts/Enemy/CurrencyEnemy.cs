using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class CurrencyEnemy : MonoBehaviour
{
	public float currencyGain;
	public MoneyManager.Currency currencyType = MoneyManager.Currency.money;

	void AddPlayerMoney()
	{
		MoneyManager moneyManager = MoneyManager.Instance;

		moneyManager.AddCurrency(currencyGain, currencyType);

		Utils.SpawnFloatingText(GetComponent<Enemy>().deathMoneyText, transform.position, MoneyManager.GetMoneyString(moneyManager.CalculateCurrency(currencyGain), currencyType), 6f, 0.3f, 40f, 0.45f, 0.15f, MoneyManager.GetCurrencyColor(currencyType));
	}

	private void OnEnable()
	{
		GetComponent<Enemy>().OnCircleHit += AddPlayerMoney;
	}

	private void OnDisable()
	{
		GetComponent<Enemy>().OnCircleHit -= AddPlayerMoney;
	}
}
