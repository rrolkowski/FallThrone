using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
	// Definicja typu obiektu
	public enum ObjectType
	{
		NotAssigned,
		Player,
		Enemy,
		Tower,
		Core
	}

	// Zmienna, w której ustawiamy typ obiektu
	public ObjectType objectType;

	// Maksymalne zdrowie obiektu
	public float maxHealth = 100f;
	// Aktualne zdrowie obiektu
	public float currentHealth;

	// Slider do wyœwietlania stanu zdrowia
	[SerializeField] private Slider healthSlider;

	// Start is called before the first frame update
	void Start()
	{
		// Na pocz¹tku ustawiamy zdrowie na maksymalne
		currentHealth = maxHealth;

		// Inicjalizacja slidera, jeœli jest przypisany
		if (healthSlider != null)
		{
			healthSlider.maxValue = maxHealth;
			healthSlider.value = currentHealth;
			// Ustawienie widocznoœci slidera, ukrycie, jeœli zdrowie jest pe³ne
			healthSlider.gameObject.SetActive(currentHealth < maxHealth);
		}
	}

	// Update is called once per frame
	void Update()
	{
		// Przyk³adowa logika: Sprawdzanie, czy obiekt nie zgin¹³
		if (currentHealth <= 0)
		{
			Die();
		}

		// Aktualizacja slidera w zale¿noœci od zdrowia
		if (healthSlider != null)
		{
			healthSlider.value = currentHealth;
			// Ukrycie slidera, gdy zdrowie jest pe³ne, pokazanie w przeciwnym razie
			healthSlider.gameObject.SetActive(currentHealth < maxHealth);
		}
	}

	// Funkcja zadawania obra¿eñ
	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		Debug.Log($"{objectType} otrzyma³ {damage} obra¿eñ. Aktualne zdrowie: {currentHealth}");

		// Aktualizacja slidera po otrzymaniu obra¿eñ
		if (healthSlider != null)
		{
			healthSlider.value = currentHealth;
			// Ukrycie slidera, gdy zdrowie jest pe³ne, pokazanie w przeciwnym razie
			healthSlider.gameObject.SetActive(currentHealth < maxHealth);
		}

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	// Funkcja obs³uguj¹ca œmieræ obiektu
	void Die()
	{
		// Specjalna logika œmierci dla ró¿nych typów obiektów
		switch (objectType)
		{
			// Logika dla Player
			case ObjectType.Player:
				Destroy(gameObject);

				break;

			// Logika dla Enemy
			case ObjectType.Enemy:
				Destroy(gameObject);
				EconomyManager.Instance.points += 2137;

				break;

			// Logika dla Tower
			case ObjectType.Tower:
				Destroy(gameObject);

				break;

			// Logika dla Core
			case ObjectType.Core:
				Destroy(gameObject);

				break;

			default:
				break;
		}
	}
}
