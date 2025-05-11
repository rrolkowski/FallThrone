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

		PlayerController.Instance.PowerUpEffect("speed", true);

		Debug.Log($"Effect start! {movement.moveSpeed}");

		AudioManager.PlaySound(SoundType.GAME_SprintPowerUpEffect);

		yield return new WaitForSeconds(_powerUpDuration);

        GameController.Instance.ClearPowerUp();
        movement.SetMoveSpeed(originalMoveSpeed);

		PlayerController.Instance.PowerUpEffect("speed", false);

		Debug.Log($"Effect end! {movement.moveSpeed}");

        Destroy(gameObject);      
    }

	protected override SoundType GetPickupSound()
	{
		return SoundType.GAME_PowerUp_Speed; // <- albo inny pasuj¹cy dŸwiêk
	}
}
