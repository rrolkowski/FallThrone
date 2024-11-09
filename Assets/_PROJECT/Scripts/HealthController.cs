using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    private System.Action<Unit> _returnToPool;
    // Definicja typu obiektu
    public enum ObjectType
	{
		NotAssigned,
		Player,
		Enemy,
		Tower,
		Core
	}

	// Zmienna, w kt�rej ustawiamy typ obiektu
	public ObjectType objectType;

	// Maksymalne zdrowie obiektu
	public float maxHealth = 100f;
	// Aktualne zdrowie obiektu
	public float currentHealth;

	// Slider do wy�wietlania stanu zdrowia
	[SerializeField] private Slider healthSlider;

    public void Init(System.Action<Unit> returnToPool)
    {
        _returnToPool = returnToPool;
    }
    // Start is called before the first frame update
    void Start()
	{
		// Na pocz�tku ustawiamy zdrowie na maksymalne
		currentHealth = maxHealth;

		// Inicjalizacja slidera, je�li jest przypisany
		if (healthSlider != null)
		{
			healthSlider.maxValue = maxHealth;
			healthSlider.value = currentHealth;
			// Ustawienie widoczno�ci slidera, ukrycie, je�li zdrowie jest pe�ne
			healthSlider.gameObject.SetActive(currentHealth < maxHealth);
		}
	}

	// Update is called once per frame
	void Update()
	{
		// Przyk�adowa logika: Sprawdzanie, czy obiekt nie zgin��
		if (currentHealth <= 0)
		{
			Die();
		}

		// Aktualizacja slidera w zale�no�ci od zdrowia
		if (healthSlider != null)
		{
			healthSlider.value = currentHealth;
			// Ukrycie slidera, gdy zdrowie jest pe�ne, pokazanie w przeciwnym razie
			healthSlider.gameObject.SetActive(currentHealth < maxHealth);
		}
	}

	// Funkcja zadawania obra�e�
	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		Debug.Log($"{objectType} otrzyma� {damage} obra�e�. Aktualne zdrowie: {currentHealth}");

		// Aktualizacja slidera po otrzymaniu obra�e�
		if (healthSlider != null)
		{
			healthSlider.value = currentHealth;
			// Ukrycie slidera, gdy zdrowie jest pe�ne, pokazanie w przeciwnym razie
			healthSlider.gameObject.SetActive(currentHealth < maxHealth);
		}

		if (currentHealth <= 0)
		{
			Die();
		}
	}
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            healthSlider.gameObject.SetActive(currentHealth < maxHealth);
        }
        Debug.Log($"Reset health for {name}. Current health: {currentHealth}");
    }
    // Funkcja obs�uguj�ca �mier� obiektu
    void Die()
	{
		// Specjalna logika �mierci dla r�nych typ�w obiekt�w
		switch (objectType)
		{
			// Logika dla Player
			case ObjectType.Player:
				Destroy(gameObject);

				break;

			// Logika dla Enemy
			case ObjectType.Enemy:
                if (TryGetComponent<Unit>(out var unit))
                {
                    Debug.Log("Invoking ReturnToPool for: " + unit.name);
                    _returnToPool?.Invoke(unit);
                }
                else
                {
                    Debug.LogWarning("Unit component not found, destroying object.");
                    Destroy(gameObject);
                }
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
