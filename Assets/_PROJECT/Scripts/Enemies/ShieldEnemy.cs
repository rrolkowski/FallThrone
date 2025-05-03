using UnityEngine;

public class ShieldEnemy : EnemyBase
{
    [SerializeField] private GameObject _shieldVisual;
    private bool shieldBroken = false;

    public override bool HasShield => !shieldBroken;

    public bool IsShieldActive => !shieldBroken;

    private void Start()
    {
        if (_shieldVisual != null)
            _shieldVisual.SetActive(true);
    }

    public void BreakShield()
    {
        if (shieldBroken) return;

        shieldBroken = true;
        if (_shieldVisual != null)
            _shieldVisual.SetActive(false);

        Debug.Log($"{name}: Tarcza zniszczona!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (shieldBroken) return;

        if (collision.gameObject.CompareTag("ShieldBreaker"))
        {
            BreakShield();
        }
    }
}
