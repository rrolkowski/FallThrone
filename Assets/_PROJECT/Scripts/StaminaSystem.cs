using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
	[Header("Stamina Settings")]
	[SerializeField] private float _maxStamina = 100f;
	[SerializeField] private float _minStamina = 0f;
	[SerializeField] private float _decreaseRate = 20f;
	[SerializeField] private float _increaseRate = 5f;

	private float _stamina;

    [Header("Modifiers")]
    [SerializeField] private bool _usePickupCost = false;
    [SerializeField] private float _pickupCost = 2.5f;

    [SerializeField] private bool _useRegenDelay = false;
    [SerializeField] private float _regenDelay = 1f;
    private float _regenTimer = 0f;

    private bool _wasHoldingLastFrame = false;

    [Header("UI Elements")]
	[SerializeField] private Slider _staminaSlider;

	void Start()
	{
		_stamina = _maxStamina;

		if (_staminaSlider != null)
		{
			_staminaSlider.minValue = _minStamina;
			_staminaSlider.maxValue = _maxStamina;
			_staminaSlider.value = _stamina;
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
            ObjectGrabber.Instance.ReleaseObject();
        }

        if (_staminaSlider != null)
        {
            _staminaSlider.value = _stamina;
        }

        _wasHoldingLastFrame = isHolding;
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
