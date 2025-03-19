using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public void OnUsePowerUp(InputAction.CallbackContext context)
    {
        GameController.Instance.ActivatePowerUp();
    }
}
