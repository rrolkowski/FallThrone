using System.Collections;
using UnityEngine;

public class SpeedBoost : BasePowerUp
{
    [SerializeField] private float _speedMultiplier = 1.5f;
    [SerializeField] private float _powerUpDuration = 5f;

    public override void ApplyEffect()
    {
        PlayerMovementController movement = FindFirstObjectByType<PlayerMovementController>();
        if (movement != null)
        {
            GameController.Instance.StartPowerUpTimer(_powerUpDuration);
            StartCoroutine(SpeedBoostCoroutine(movement));
        }
    }

    IEnumerator SpeedBoostCoroutine(PlayerMovementController movement)
    {       
        float originalMoveSpeed = movement.GetRawMoveSpeed();
        float boostedMoveSpeed = originalMoveSpeed * _speedMultiplier;
        movement.SetMoveSpeed(boostedMoveSpeed);
        Debug.Log($"Effect start! {movement.moveSpeed}");

        yield return new WaitForSeconds(_powerUpDuration);

        GameController.Instance.ClearPowerUp();
        movement.SetMoveSpeed(originalMoveSpeed);
        Debug.Log($"Effect end! {movement.moveSpeed}");

        Destroy(gameObject);      
    }
}
