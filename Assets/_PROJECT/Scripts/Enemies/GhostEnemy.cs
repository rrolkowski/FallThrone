using UnityEngine;

public class GhostEnemy : EnemyBase
{
    [SerializeField] private ParticleSystem _lockParticles;
    public override bool CanBeGrabbed => !_wasThrown;

    public override void OnThrown()
    {
        base.OnThrown();
        if (_lockParticles != null)
            _lockParticles.Play();
    }
}
