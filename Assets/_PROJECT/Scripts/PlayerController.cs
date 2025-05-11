using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
	public Transform speed;
	public Transform range;

	private void Awake()
	{
		Instance = this;
	}

	public void OnUsePowerUp(InputAction.CallbackContext context)
    {
        GameController.Instance.ActivatePowerUp();
    }

    public void PowerUpEffect(string powerup, bool enable)
	{
		if (powerup == "speed")
		{
			if (enable)
			{
				speed.gameObject.SetActive(true);
			}
            else
			{
				speed.gameObject.SetActive(false);
			}
        }
		if (powerup == "range")
		{
			if (enable)
			{
				range.gameObject.SetActive(true);
			}
			else
			{
				range.gameObject.SetActive(false);
			}
		}
	}
}
