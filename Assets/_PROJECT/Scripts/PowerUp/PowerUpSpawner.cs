using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject[] _spawnPoints;
    [SerializeField] private GameObject[] _powerUpPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] float _minTimeBtwSpawns;
    [SerializeField] float _maxTimeBtwSpawns;

    private bool _powerUpActive = false;

    private void Start()
    {
        Invoke(nameof(SpawnPowerUp), Random.Range(5f, 10f));
    }

    void SpawnPowerUp()
    {
        if (_powerUpActive) return;

        var spawnPointIndex = Random.Range(0, _spawnPoints.Length);
        var currentPoint = _spawnPoints[spawnPointIndex];

        int powerUpIndex = Random.Range(0, _powerUpPrefabs.Length);

        float timeBtwSpawns = Random.Range(_minTimeBtwSpawns, _maxTimeBtwSpawns);

        GameObject spawnedPowerUp = Instantiate(_powerUpPrefabs[powerUpIndex], currentPoint.transform.position, Quaternion.Euler(60, 0, 0));

        _powerUpActive = true;

        GameController.Instance.OnPickedUp += HandlePowerUpPickedUp;
    }

    void HandlePowerUpPickedUp()
    {
        float timeBtwSpawns = Random.Range(_minTimeBtwSpawns, _maxTimeBtwSpawns);

        _powerUpActive = false;
        Debug.Log(timeBtwSpawns);
        Invoke(nameof(SpawnPowerUp), timeBtwSpawns);

        GameController.Instance.OnPickedUp -= HandlePowerUpPickedUp;
    }
}
