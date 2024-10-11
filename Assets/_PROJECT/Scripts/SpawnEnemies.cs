using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
	public static SpawnEnemies Instance { get; private set; }

	public float spawnDelay = 1f;
	public GameObject enemyPrefab;

	public Transform enemiesHolder;
	public Transform enemiesSpawnPoints;

	private List<Transform> spawnpoints = new List<Transform>();

	private void Awake()
	{
		Instance = this;

		foreach (Transform child in enemiesSpawnPoints)
		{
			spawnpoints.Add(child);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.U))
		{
			StartCoroutine(SpawnEnemiesCoroutine(2));
		}
	}

	public IEnumerator SpawnEnemiesCoroutine(int numberOfEnemiesToSpawn)
	{
		for (int i = 0; i < numberOfEnemiesToSpawn; i++)
		{
			if (spawnpoints.Count == 0)
			{
				Debug.Log("No more spawn points available.");
				yield break;
			}

			int randomIndex = Random.Range(0, spawnpoints.Count);
			Transform spawnpoint = spawnpoints[randomIndex];

			GameObject newEnemy = Instantiate(enemyPrefab, spawnpoint.position, spawnpoint.rotation);

			newEnemy.transform.SetParent(enemiesHolder.transform);

			spawnpoints.RemoveAt(randomIndex);

			yield return new WaitForSeconds(spawnDelay);
		}
	}
}
