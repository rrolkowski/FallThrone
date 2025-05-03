using UnityEngine;

public class TankEnemy : EnemyBase
{
    [SerializeField] private float staminaMultiplier = 2f;
    [SerializeField] private float speedMultiplierWhileHeld = 0.5f;

    private float originalPlayerSpeed;

    public override void OnGrabbed()
    {
        base.OnGrabbed();

        // Spowolnienie gracza
        var player = FindFirstObjectByType<PlayerMovementController>();
        if (player != null)
        {
            originalPlayerSpeed = player.GetRawMoveSpeed();
            player.SetMoveSpeed(originalPlayerSpeed * speedMultiplierWhileHeld);
        }
    }

    public override void OnReleased()
    {
        base.OnReleased();

        // Przywrócenie prêdkoœci gracza
        var player = FindFirstObjectByType<PlayerMovementController>();
        if (player != null)
        {
            player.SetMoveSpeed(originalPlayerSpeed);
        }
    }

    // Dodatkowa metoda do u¿ycia w StaminaSystem lub ObjectGrabber
    public float GetStaminaMultiplier() => staminaMultiplier;
}
