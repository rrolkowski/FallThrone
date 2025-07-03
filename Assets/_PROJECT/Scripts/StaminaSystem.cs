using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaSystem : MonoBehaviour
{
	[Header("Stamina Settings")]
	[SerializeField] private float _maxStamina = 100f;
	[SerializeField] private float _minStamina = 0f;
	[SerializeField] private float _decreaseRate = 10f;
	[SerializeField] private float _increaseRate = 15f;

	private float _stamina;

	[Header("Modifiers")]
	[SerializeField] private bool _usePickupCost = true;
	[SerializeField] private float _pickupCost = 2.5f;

	[SerializeField] private bool _useRegenDelay = true;
	[SerializeField] private float _regenDelay = 1f;
	private float _regenTimer = 0f;

	private bool _wasHoldingLastFrame = false;

	[Header("UI Elements")]
	[SerializeField] private Slider _staminaSlider;
	[SerializeField] private Image _staminaFillImage;
	[SerializeField] private TextMeshProUGUI _staminaText;

	[Header("Color Settings")]
	[SerializeField] private Color fullStaminaColor = Color.white;
	[SerializeField] private Color lowStaminaColor = Color.red;
	[SerializeField] private float colorChangeStartPercent = 30f;
	[SerializeField] private float fullRedPercent = 10f;

	[Header("Pulse Settings (for Text)")]
	[SerializeField] private bool enablePulse = true;
	[SerializeField] private float maxPulseSpeed = 20f;
	[SerializeField] private float maxPulseScaleAmount = 0.01f;

	private Vector3 originalTextScale;

	// NEW: Cooldown for stamina warning sound
	private float _noStaminaSoundCooldown = 0f;
	[SerializeField] private float _noStaminaSoundInterval = .5f;
    private bool _hasPlayedNoStaminaSound = false;

    void Start()
	{
		_stamina = _maxStamina;

		if (_staminaSlider != null)
		{
			_staminaSlider.minValue = _minStamina;
			_staminaSlider.maxValue = _maxStamina;
			_staminaSlider.value = _stamina;
		}

		if (_staminaFillImage != null)
		{
			_staminaFillImage.color = fullStaminaColor;
		}

		if (_staminaText != null)
		{
			_staminaText.color = fullStaminaColor;
			originalTextScale = _staminaText.rectTransform.localScale;
		}
	}

	void Update()
	{
		bool isHolding = ObjectGrabber.Instance.isHoldingObject;

		if (isHolding)
		{
			float multiplier = 1f;

			if (ObjectGrabber.Instance.currentlyGrabbedObject != null &&
				ObjectGrabber.Instance.currentlyGrabbedObject.TryGetComponent<EnemyBase>(out var enemy))
			{
				if (enemy is TankEnemy tank)
					multiplier = tank.GetStaminaMultiplier();
			}

			_stamina -= _decreaseRate * multiplier * Time.deltaTime;

			if (_useRegenDelay)
				_regenTimer = _regenDelay;
		}
		else
		{
			if (_useRegenDelay && _wasHoldingLastFrame)
				_regenTimer = _regenDelay;

			if (_regenTimer > 0f)
			{
				_regenTimer -= Time.deltaTime;
			}
			else
			{
				_stamina += _increaseRate * Time.deltaTime;
			}
		}

		_stamina = Mathf.Clamp(_stamina, _minStamina, _maxStamina);

		if (_stamina <= _minStamina && isHolding)
		{
			ObjectGrabber.Instance.ThrowObject();
		}

		if (_staminaSlider != null)
		{
			_staminaSlider.value = _stamina;
		}

		UpdateStaminaUI();

		_wasHoldingLastFrame = isHolding;

		// Update cooldown timer
		if (_noStaminaSoundCooldown > 0f)
		{
			_noStaminaSoundCooldown -= Time.deltaTime;
		}
	}

	private void UpdateStaminaUI()
	{
		if (_staminaFillImage == null)
			return;

		float staminaPercent = (_stamina / _maxStamina) * 100f;
		Color targetColor = fullStaminaColor;

		if (staminaPercent <= colorChangeStartPercent)
		{
			// Play sound only if cooldown expired
			if (!_hasPlayedNoStaminaSound)
			{
				AudioManager.PlaySound(SoundType.GAME_NoStamina);
                _hasPlayedNoStaminaSound = true;
            }

			float t = Mathf.InverseLerp(colorChangeStartPercent, fullRedPercent, staminaPercent);
			t = Mathf.Clamp01(t);
			targetColor = Color.Lerp(fullStaminaColor, lowStaminaColor, t);

			if (enablePulse && _staminaText != null)
			{
				ApplyTextPulse(t);
			}
		}
		else
		{
			ResetTextScale();
            _hasPlayedNoStaminaSound = false;
        }

		_staminaFillImage.color = targetColor;

		if (_staminaText != null)
		{
			_staminaText.color = targetColor;
		}
	}

	private void ApplyTextPulse(float pulseIntensity)
	{
		if (_staminaText == null)
			return;

		float pulseSpeed = maxPulseSpeed;
		float pulseScaleAmount = maxPulseScaleAmount;

		float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScaleAmount;
		_staminaText.rectTransform.localScale = originalTextScale * scale;
	}

	private void ResetTextScale()
	{
		if (_staminaText != null)
		{
			_staminaText.rectTransform.localScale = originalTextScale;
		}
	}

	public bool TryConsumePickupCost()
	{
		if (!_usePickupCost)
			return true;

		if (_stamina >= _pickupCost)
		{
			_stamina -= _pickupCost;
			return true;
		}

		return false;
	}

	public bool ConsumeSprintStamina(float amount)
	{
		if (_stamina <= _minStamina)
			return false;

		_stamina -= amount * Time.deltaTime;

		if (_useRegenDelay)
			_regenTimer = _regenDelay;

		_stamina = Mathf.Clamp(_stamina, _minStamina, _maxStamina);
		return true;
	}
}
