using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class EnemyBase : MonoBehaviour
{
    protected HealthController _health;
    protected Rigidbody _rb;

    public virtual bool CanBeGrabbed => true;

    protected bool _wasThrown = false;

    public virtual bool HasShield => false;

    public virtual void OnThrown() => _wasThrown = true;

    public bool WasThrown => _wasThrown;

    protected virtual void Awake()
    {
        _health = GetComponent<HealthController>();
        _rb = GetComponent<Rigidbody>();
    }

    public virtual void OnGrabbed() { }

    public virtual void OnReleased() { }
}
