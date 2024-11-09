using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class UnitSpawner : MonoBehaviour
{
    private ObjectPool<Unit> _enemyPool;

    [SerializeField] PathFinderManager pathfindingManager;

    [SerializeField] Unit _unitEnemyPrefab;

    [SerializeField] Transform _unitParent;

    [SerializeField] int _maxEnemyUnits;

    [SerializeField] bool _usePool;

    [SerializeField] float _minSpawnDelay = 1f;
    [SerializeField] float _maxSpawnDelay = 3f;

    private int _spawnedUnits = 0;
    private int _currentSpawnedUnits = 0;
    

    private void Start()
    {
        _enemyPool = new ObjectPool<Unit>(() =>
        {
            Debug.Log("Create POOL");
            var unit = Instantiate(_unitEnemyPrefab);
            unit.transform.SetParent(_unitParent);
            return unit;
        }, unit =>
        {
            unit.gameObject.SetActive(true);
            unit.ResetValues();
            Debug.Log("Get POOL");
        }, unit =>
        {
            unit.gameObject.SetActive(false);
            Debug.Log("Release POOL");
        }, unit =>
        {
            Destroy(unit.gameObject);
        }, false, 1, _maxEnemyUnits); StartCoroutine(SpawnUnitLoop());

    }

    private IEnumerator SpawnUnitLoop()
    {
        while (_currentSpawnedUnits < _maxEnemyUnits && _spawnedUnits <= _maxEnemyUnits)
        {
            SpawnUnit();

            yield return new WaitForSeconds(Random.Range(_minSpawnDelay, _maxSpawnDelay));
        }

    }
    private void SpawnUnit()
    {
        if (_spawnedUnits < _maxEnemyUnits)
        {
            var unit = _usePool ? _enemyPool.Get() : Instantiate(_unitEnemyPrefab);

            Vector3 spawnPosition = pathfindingManager.gridManager.tilemap.GetCellCenterWorld(pathfindingManager.startPoint);
            float yOffset = transform.localScale.y;
            unit.transform.position = new Vector3(spawnPosition.x, spawnPosition.y + yOffset, spawnPosition.z);

            unit.Init(ReturnUnitToPool);

            if (unit.TryGetComponent<HealthController>(out var healthController))
            {
                healthController.Init(ReturnUnitToPool);
            }
            _currentSpawnedUnits++;
            _spawnedUnits++;
        }
    }

    private void ReturnUnitToPool(Unit unit)
    {
        Debug.Log("Returning unit: " + unit.name);

        if (_usePool)
        {
            Debug.Log($"Returning unit to pool: {unit.name}");
            _enemyPool.Release(unit);
            _currentSpawnedUnits--;
        }
        else
        {
            Destroy(unit.gameObject);
        }
    }

}

