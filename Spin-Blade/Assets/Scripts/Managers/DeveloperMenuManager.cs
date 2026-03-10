using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeveloperMenuManager : MonoBehaviour
{
	public bool developmentMode = true;

	[Space(10)]

	public GameObject developerMenuUI;

	[Header("UI Input Fields")]
	public TMP_InputField takeDamageField;
	public TMP_InputField healField;
	public TMP_InputField addMoneyField;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Keyboard.current.dKey.wasPressedThisFrame && developmentMode)
		{
			ToggleDeveloperMenu();
		}
	}

	public void ToggleDeveloperMenu()
	{
		developerMenuUI.SetActive(!developerMenuUI.activeSelf);
	}

	public void AddMoney()
	{
		float moneyToAdd = float.Parse(addMoneyField.text);

		MoneyManager.Instance.AddCurrency(moneyToAdd);
	}

	public void TakeDamage()
	{
		float damageToTake = float.Parse(takeDamageField.text);
		PlayerHealthAndDamage playerHealthAndDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

		playerHealthAndDamage.TakeDamage(damageToTake);
	}

	public void Heal()
	{
		float healAmount = float.Parse(healField.text);
		PlayerHealthAndDamage playerHealthAndDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

		playerHealthAndDamage.Heal(healAmount);
	}

	public void KillAllEnemies()
	{

		EnemyManager enemyManager = EnemyManager.Instance;
		foreach (Enemy enemy in enemyManager.enemies)
		{
			enemy.Death(false);
		}
	}

	public void TriggerEvent()
	{
		EventManager eventManager = EventManager.Instance;

		eventManager.StartRandomEvent();
	}
}
