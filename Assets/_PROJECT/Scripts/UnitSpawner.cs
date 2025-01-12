using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Unity.Cinemachine;

public class UnitSpawner : MonoBehaviour
{
	public static UnitSpawner Instance;

	private ObjectPool<Unit> _enemyPool;

	[SerializeField] private Unit _unitEnemyPrefab;

	[SerializeField] private Transform _unitParent;

	[SerializeField] public int _maxEnemyUnits;

	[SerializeField] private bool _usePool;

	[SerializeField] private float _minSpawnDelay = 1f;
	[SerializeField] private float _maxSpawnDelay = 3f;

	[SerializeField] private CinemachineTargetGroup _targetGroup; // Dodano Cinemachine Target Group

	public int _spawnedUnits = 0;
	private int _currentSpawnedUnits = 0;

	private void Awake()
	{
		Instance = this;
	}

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
			AddToTargetGroup(unit); // Dodaj do Cinemachine Target Group
		}, unit =>
		{
			unit.gameObject.SetActive(false);
			RemoveFromTargetGroup(unit); // Usuñ z Cinemachine Target Group
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
			yield return new WaitForSeconds(Random.Range(_minSpawnDelay, _maxSpawnDelay));
			SpawnUnit();
		}
	}

	private void SpawnUnit()
	{
		if (_spawnedUnits < _maxEnemyUnits)
		{
			var unit = _usePool ? _enemyPool.Get() : Instantiate(_unitEnemyPrefab);

			var spawnPositions = PathFinderManager.Instance.GetSpawnPositions();
			if (spawnPositions.Count == 0)
			{
				Debug.LogError("Brak kafelków spawnTile! Dodaj co najmniej jeden spawnTile.");
				return;
			}

			Vector3Int spawnPoint = spawnPositions[Random.Range(0, spawnPositions.Count)];
			Vector3 spawnPosition = PathFinderManager.Instance.gridManager.tilemap.GetCellCenterWorld(spawnPoint);
			float yOffset = transform.localScale.y;
			unit.transform.position = new Vector3(spawnPosition.x, spawnPosition.y + yOffset, spawnPosition.z);

			List<TileNode> path = PathFinderManager.Instance.GetPathFromSpawnPoint(spawnPoint);

			unit.Init(ReturnUnitToPool);
			if (unit.TryGetComponent<EnemyMovement>(out var movement))
			{
				if (path == null || path.Count == 0)
				{
					Debug.LogError($"Nie uda³o siê znaleŸæ œcie¿ki od punktu spawn {spawnPoint}");
				}
				else
				{
					movement.SetPath(path);
				}
			}

			if (unit.TryGetComponent<HealthController>(out var healthController))
			{
				healthController.Init(ReturnUnitToPool);
			}

			_currentSpawnedUnits++;
			_spawnedUnits++;
		}
		GameController.Instance.CheckForWinCondition();
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
			RemoveFromTargetGroup(unit); // Usuñ z Cinemachine Target Group
			Destroy(unit.gameObject);
		}
	}

	private void AddToTargetGroup(Unit unit)
	{
		if (_targetGroup != null)
		{
			_targetGroup.AddMember(unit.transform, 1f, 0.5f); // Waga i promieñ mo¿na dostosowaæ
		}
	}

	private void RemoveFromTargetGroup(Unit unit)
	{
		if (_targetGroup != null)
		{
			_targetGroup.RemoveMember(unit.transform);
		}
	}
}
