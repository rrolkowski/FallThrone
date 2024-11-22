using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
	public float stamina = 100f; // Aktualna wartoœæ staminy
	public float maxStamina = 100f; // Maksymalna wartoœæ staminy
	public float minStamina = 0f; // Minimalna wartoœæ staminy
	public float decreaseRate = 10f; // Szybkoœæ spadku staminy
	public float increaseRate = 5f; // Szybkoœæ wzrostu staminy

	[Header("UI Elements")]
	[SerializeField] private Slider staminaSlider; // Odniesienie do suwaka

	void Start()
	{
		if (staminaSlider != null)
		{
			staminaSlider.minValue = minStamina;
			staminaSlider.maxValue = maxStamina;
			staminaSlider.value = stamina; // Ustaw pocz¹tkow¹ wartoœæ
		}
	}

	void Update()
	{
		// U¿yj ObjectGrabber.Instance.isHoldingObject
		if (ObjectGrabber.Instance.isHoldingObject)
		{
			stamina -= decreaseRate * Time.deltaTime;
		}
		else
		{
			stamina += increaseRate * Time.deltaTime;
		}

		// Ograniczanie wartoœci staminy
		stamina = Mathf.Clamp(stamina, minStamina, maxStamina);

		// SprawdŸ, czy stamina spad³a do 0
		if (stamina <= minStamina && ObjectGrabber.Instance.isHoldingObject)
		{
			ObjectGrabber.Instance.ThrowObject();
		}

		// Aktualizuj wartoœæ suwaka
		if (staminaSlider != null)
		{
			staminaSlider.value = stamina;
		}
	}
}
