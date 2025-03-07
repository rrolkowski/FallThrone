using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeTower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _shockwaveSpawnPoint;
    [SerializeField] private GameObject _shockwaveEffect;  // Currently PH - (To change)

    [Header("AOE Values")]
    [SerializeField] private float _fireRate = 1f;
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private float _shockwaveRadius = 10f;
    [SerializeField] private float _shockwaveDamage = 50f;
    [SerializeField] private float _shockwaveExpansionTime = 1f;

    private float _nextFireTime = 0f;
    private float _currentRadius = 0f;

    private HashSet<GameObject> _hitEnemies = new HashSet<GameObject>();

    void Update()
    {
        if (Time.time >= _nextFireTime && DetectEnemiesInRange())
        {
            StartCoroutine(CastShockwave());
            _nextFireTime = Time.time + _fireRate;
        }
    }

    // Start dealing AOE damage when enemies are detected
    private bool DetectEnemiesInRange()
    {
        Collider[] hit_colliders = Physics.OverlapSphere(transform.position, _detectionRange);
        foreach (var collider in hit_colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    // Shockwave
    private IEnumerator CastShockwave()
    {
        Vector3 spawn_position = _shockwaveSpawnPoint != null ? _shockwaveSpawnPoint.position : transform.position;
        GameObject shockwave = Instantiate(_shockwaveEffect, spawn_position, Quaternion.identity);
        //AUDIO -- New audio needed for Minigun Projectiles
        //AudioManager.PlaySound(SoundType.GAME_Turret_Shockwave);

        _hitEnemies.Clear();

        float elapsedTime = 0f;
        _currentRadius = 0f;

        while (elapsedTime < _shockwaveExpansionTime)
        {
            elapsedTime += Time.deltaTime;
            _currentRadius = Mathf.Lerp(0, _shockwaveRadius, elapsedTime / _shockwaveExpansionTime);

            Collider[] hit_colliders = Physics.OverlapSphere(spawn_position, _currentRadius);

            foreach (var collider in hit_colliders)
            {
                if (collider.CompareTag("Enemy") && !_hitEnemies.Contains(collider.gameObject))
                {
                    HealthController health = collider.GetComponent<HealthController>();
                    if (health != null && health.objectType == HealthController.ObjectType.Enemy)
                    {
                        health.TakeDamage(_shockwaveDamage);
                        _hitEnemies.Add(collider.gameObject);
                    }
                }
            }
            yield return null;
        }

        Destroy(shockwave, _shockwaveExpansionTime);
    }


    // Visualisation
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        if (_shockwaveSpawnPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_shockwaveSpawnPoint.position, _currentRadius);
        }
    }
}
