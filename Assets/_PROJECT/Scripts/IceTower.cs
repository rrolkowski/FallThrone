using UnityEngine;
using static Tower;

public class IceTower : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _spawnPoint;

    [Header("Values")]
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private float _fireRate = 1f;

    private float nextFireTime = 0f;

    [SerializeField] private TargetingStrategy targetingStrategy = TargetingStrategy.Closest;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            GameObject targetEnemy = FindTargetEnemy();
            if (targetEnemy != null)
            {
                Shoot(targetEnemy);

                nextFireTime = Time.time + _fireRate;
            }
        }
    }

    GameObject FindTargetEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject selectedEnemy = null;
        float bestValue = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance > _detectionRange) continue;

            switch (targetingStrategy)
            {
                case TargetingStrategy.Closest:
                    if (distance < bestValue)
                    {
                        bestValue = distance;
                        selectedEnemy = enemy;
                    }
                    break;

                case TargetingStrategy.LowestHP:
                    if (enemy.TryGetComponent(out HealthController hp))
                    {
                        if (hp.currentHealth < bestValue)
                        {
                            bestValue = hp.currentHealth;
                            selectedEnemy = enemy;
                        }
                    }
                    break;
            }
        }

        return selectedEnemy;
    }

    void Shoot(GameObject target)
    {
        if (_spawnPoint != null)
        {
            //AUDIO -- New audio needed for Ice Projectiles
            AudioManager.PlaySound(SoundType.GAME_Turret_Ice);

            GameObject projectile = Instantiate(_projectilePrefab, _spawnPoint.position, _spawnPoint.rotation);

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetTarget(target.transform);
            }
        }
        else
        {
            Debug.LogWarning("SpawnPoint nie jest przypisany!");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }

    public enum TargetingStrategy
    {
        Closest,
        LowestHP
    }
}
