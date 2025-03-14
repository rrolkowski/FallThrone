using UnityEngine;

public class IceTower : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _spawnPoint;

    [Header("Values")]
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private float _fireRate = 1f;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            GameObject nearestEnemy = FindClosestEnemy();
            if (nearestEnemy != null)
            {
                Shoot(nearestEnemy);

                nextFireTime = Time.time + _fireRate;
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = _detectionRange;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
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
}
