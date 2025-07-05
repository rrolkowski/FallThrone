using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Unity.Cinemachine;
using TMPro;

[System.Serializable]
public class EnemySpawnData
{
	public Unit prefab;
	public int count;
}

public class UnitSpawner : MonoBehaviour
{
	public static UnitSpawner Instance;

	[SerializeField] private List<EnemySpawnData> enemySpawnConfigs;
	[SerializeField] private Transform _unitParent;
	[SerializeField] public int _maxEnemyUnits;
	[SerializeField] private bool _usePool;
	[SerializeField] private float _initialSpawnDelay = 0f;
	[SerializeField] private float _minSpawnDelay = 1f;
	[SerializeField] private float _maxSpawnDelay = 3f;
	[SerializeField] private CinemachineTargetGroup _targetGroup;
	[SerializeField] private TextMeshProUGUI _enemyCounterText;
	[SerializeField] private List<GameObject> _spawnIndicatorsUI;
	[SerializeField] private float _indicatorDuration = 2f;

	public int _spawnedUnits = 0;
	private int _currentSpawnedUnits = 0;

	private Dictionary<string, ObjectPool<Unit>> enemyPools;
	private Dictionary<string, Unit> prefabLookup;
	private Dictionary<string, int> remainingCounts;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		enemyPools = new Dictionary<string, ObjectPool<Unit>>();
		prefabLookup = new Dictionary<string, Unit>();
		remainingCounts = new Dictionary<string, int>();

		int totalAvailable = 0;

		foreach (var config in enemySpawnConfigs)
		{
			string prefabName = config.prefab.name;
			prefabLookup[prefabName] = config.prefab;
			remainingCounts[prefabName] = config.count;
			totalAvailable += config.count;

			var pool = new ObjectPool<Unit>(() =>
			{
				var unit = Instantiate(config.prefab, _unitParent);
				return unit;
			}, unit =>
			{
				unit.gameObject.SetActive(true);
				unit.ResetValues();
				AddToTargetGroup(unit);
			}, unit =>
			{
				unit.gameObject.SetActive(false);
				RemoveFromTargetGroup(unit);
			}, unit =>
			{
				Destroy(unit.gameObject);
			}, false, 1, config.count);

			enemyPools[prefabName] = pool;
		}

		_maxEnemyUnits = Mathf.Min(_maxEnemyUnits, totalAvailable);

		UpdateEnemyCounter();
		StartCoroutine(WaitForTutorialEndThenSpawn());
	}

	private void Update()
	{
		UpdateEnemyCounter();
	}

	private IEnumerator WaitForTutorialEndThenSpawn()
	{
		// Jeœli tutorial jest aktywny - czekamy
		while (GameController.Instance.isTutorialActive)
		{
			yield return null;
		}

		// Tutorial siê skoñczy³ lub wcale go nie by³o - odpalamy spawn loop
		StartCoroutine(SpawnUnitLoop());
	}

	private IEnumerator SpawnUnitLoop()
	{
		yield return new WaitForSeconds(_initialSpawnDelay);

		while (_currentSpawnedUnits < _maxEnemyUnits)
		{
			SpawnUnit();
			yield return new WaitForSeconds(Random.Range(_minSpawnDelay, _maxSpawnDelay));
		}
	}

	private void SpawnUnit()
	{
		if (_spawnedUnits >= _maxEnemyUnits)
			return;

		List<string> availableTypes = new List<string>();
		foreach (var kvp in remainingCounts)
		{
			if (kvp.Value > 0)
				availableTypes.Add(kvp.Key);
		}

		if (availableTypes.Count == 0)
			return;

		string selectedName = availableTypes[Random.Range(0, availableTypes.Count)];
		remainingCounts[selectedName]--;

		var unit = _usePool
			? enemyPools[selectedName].Get()
			: Instantiate(prefabLookup[selectedName]);

		var spawnPositions = PathFinderManager.Instance.GetSpawnPositions();
		if (spawnPositions.Count == 0)
		{
			Debug.LogError("Brak kafelków spawnTile! Dodaj co najmniej jeden spawnTile.");
			return;
		}

		Vector3Int spawnPoint = spawnPositions[Random.Range(0, spawnPositions.Count)];
		int spawnIndex = spawnPositions.IndexOf(spawnPoint);
		if (spawnIndex >= 0 && spawnIndex < _spawnIndicatorsUI.Count)
		{
			StartCoroutine(ShowIndicator(spawnIndex));
            AudioManager.PlaySound(SoundType.GAME_INDICATOR);
        }

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

		UpdateEnemyCounter();
		GameController.Instance.CheckForWinCondition();
	}

	private void ReturnUnitToPool(Unit unit)
	{
		string prefabName = unit.name.Replace("(Clone)", "").Trim();
		if (_usePool && enemyPools.ContainsKey(prefabName))
		{
			enemyPools[prefabName].Release(unit);
			_currentSpawnedUnits--;
		}
		else
		{
			RemoveFromTargetGroup(unit);
			Destroy(unit.gameObject);
		}

		GameController.Instance.EnemyDefeated();
		UpdateEnemyCounter();
	}

	private void AddToTargetGroup(Unit unit)
	{
		if (_targetGroup != null)
		{
			_targetGroup.AddMember(unit.transform, 1f, 0.5f);
		}
	}

	private void RemoveFromTargetGroup(Unit unit)
	{
		if (_targetGroup != null)
		{
			_targetGroup.RemoveMember(unit.transform);
		}
	}

	private void UpdateEnemyCounter()
	{
		if (_enemyCounterText != null)
		{
			int lostHealth = GameController.Instance.maxHealth - GameController.Instance.currentHealth;
			int remainingEnemies = _maxEnemyUnits - GameController.Instance.DefeatedEnemies - lostHealth;
			remainingEnemies = Mathf.Max(remainingEnemies, 0);
			_enemyCounterText.text = $"{remainingEnemies}";
		}
	}

	private IEnumerator ShowIndicator(int index)
	{
        //var go = _spawnIndicatorsUI[index];
        //go.SetActive(true);
        //yield return new WaitForSeconds(_indicatorDuration);
        //go.SetActive(false);

        var go = _spawnIndicatorsUI[index];
        go.SetActive(true);

        float duration = _indicatorDuration;
        float speed = 1.5f; // pulsów na sekundê
        float fixedAlpha = 180f / 255f;

        SpriteRenderer sr = null;
        UnityEngine.UI.Image img = null;

        if (!go.TryGetComponent<SpriteRenderer>(out sr))
            go.TryGetComponent<UnityEngine.UI.Image>(out img);

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float wave = (Mathf.Sin(t * speed * Mathf.PI * 2f) + 1f) / 2f;
            Color lerped = Color.Lerp(Color.red, Color.white, wave);
            lerped.a = fixedAlpha;

            if (sr != null)
                sr.color = lerped;
            else if (img != null)
                img.color = lerped;

            yield return null;
        }

        // Reset kolor na bia³y z alf¹
        Color finalColor = new Color(1f, 1f, 1f, fixedAlpha);

        if (sr != null)
            sr.color = finalColor;
        else if (img != null)
            img.color = finalColor;

        go.SetActive(false);
    }
}

