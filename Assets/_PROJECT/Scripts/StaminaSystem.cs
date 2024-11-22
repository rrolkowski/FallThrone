using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
	public float stamina = 100f; // Aktualna warto�� staminy
	public float maxStamina = 100f; // Maksymalna warto�� staminy
	public float minStamina = 0f; // Minimalna warto�� staminy
	public float decreaseRate = 10f; // Szybko�� spadku staminy
	public float increaseRate = 5f; // Szybko�� wzrostu staminy

	[Header("UI Elements")]
	[SerializeField] private Slider staminaSlider; // Odniesienie do suwaka

	void Start()
	{
		if (staminaSlider != null)
		{
			staminaSlider.minValue = minStamina;
			staminaSlider.maxValue = maxStamina;
			staminaSlider.value = stamina; // Ustaw pocz�tkow� warto��
		}
	}

	void Update()
	{
		// U�yj ObjectGrabber.Instance.isHoldingObject
		if (ObjectGrabber.Instance.isHoldingObject)
		{
			stamina -= decreaseRate * Time.deltaTime;
		}
		else
		{
			stamina += increaseRate * Time.deltaTime;
		}

		// Ograniczanie warto�ci staminy
		stamina = Mathf.Clamp(stamina, minStamina, maxStamina);

		// Sprawd�, czy stamina spad�a do 0
		if (stamina <= minStamina && ObjectGrabber.Instance.isHoldingObject)
		{
			ObjectGrabber.Instance.ThrowObject();
		}

		// Aktualizuj warto�� suwaka
		if (staminaSlider != null)
		{
			staminaSlider.value = stamina;
		}
	}
}
