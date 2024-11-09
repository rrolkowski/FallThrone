using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UnitSpawner : MonoBehaviour
{
	private ObjectPool<Unit> _enemyPool;

	[SerializeField] private PathFinderManager pathfindingManager;

	[SerializeField] private Unit _unitEnemyPrefab;

	[SerializeField] private Transform _unitParent;

	[SerializeField] private int _maxEnemyUnits;

	[SerializeField] private bool _usePool;

	[SerializeField] private float _minSpawnDelay = 1f;
	[SerializeField] private float _maxSpawnDelay = 3f;

	private int _spawnedUnits = 0;
	private int _currentSpawnedUnits = 0;

	private void Start()
	{
		_enemyPool = new ObjectPool<Unit>(() =>
		{
			var unit = Instantiate(_unitEnemyPrefab);
			unit.transform.SetParent(_unitParent);
			return unit;
		}, unit =>
		{
			unit.gameObject.SetActive(true);
			unit.ResetValues();
		}, unit =>
		{
			unit.gameObject.SetActive(false);
		}, unit =>
		{
			Destroy(unit.gameObject);
		}, false, 1, _maxEnemyUnits);

		StartCoroutine(SpawnUnitLoop());
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

			// Pobierz pozycje spawnTile z PathFinderManager
			var spawnPositions = pathfindingManager.GetSpawnPositions();
			if (spawnPositions.Count == 0)
			{
				Debug.LogError("Brak kafelków spawnTile! Dodaj co najmniej jeden spawnTile.");
				return;
			}

			Vector3Int spawnPoint = spawnPositions[Random.Range(0, spawnPositions.Count)];
			Vector3 spawnPosition = pathfindingManager.gridManager.tilemap.GetCellCenterWorld(spawnPoint);
			float yOffset = transform.localScale.y;
			unit.transform.position = new Vector3(spawnPosition.x, spawnPosition.y + yOffset, spawnPosition.z);

			// Pobierz œcie¿kê od wybranego punktu spawnu
			List<TileNode> path = pathfindingManager.GetPathFromSpawnPoint(spawnPoint);

			// Przeka¿ punkt spawnu i œcie¿kê do EnemyMovement
			unit.Init(ReturnUnitToPool);
			if (unit.TryGetComponent<EnemyMovement>(out var movement))
			{
				movement.SetPath(path);
			}

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
		if (_usePool)
		{
			_enemyPool.Release(unit);
			_currentSpawnedUnits--;
		}
		else
		{
			Destroy(unit.gameObject);
		}
	}
}
