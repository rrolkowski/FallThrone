using UnityEngine;

public class Tower : MonoBehaviour
{
	public GameObject projectilePrefab;  // Prefab pocisku
	public Transform spawnPoint;         // Punkt, z którego s¹ spawnowane pociski
	public float detectionRange = 10f;   // Zasiêg wykrywania wrogów
	public float fireRate = 1f;          // Czêstotliwoœæ strza³ów (czas w sekundach miêdzy kolejnymi strza³ami)
	private float nextFireTime = 0f;     // Czas do nastêpnego strza³u

	// Update is called once per frame
	void Update()
	{
		// Sprawdzenie, czy mo¿na ju¿ wystrzeliæ
		if (Time.time >= nextFireTime)
		{
			// Szukaj najbli¿szego wroga w zasiêgu
			GameObject nearestEnemy = FindClosestEnemy();
			if (nearestEnemy != null)
			{
				// Respawn pocisku
				Shoot(nearestEnemy);
				// Ustaw czas na nastêpny strza³
				nextFireTime = Time.time + fireRate;
			}
		}
	}

	// Funkcja wyszukuj¹ca najbli¿szego wroga w zasiêgu
	GameObject FindClosestEnemy()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject closestEnemy = null;
		float closestDistance = detectionRange;

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

	// Funkcja strzelaj¹ca w kierunku wroga
	void Shoot(GameObject target)
	{
		if (spawnPoint != null)
		{
			//AUDIO
			AudioManager.PlaySound(SoundType.GAME_Turret_Fireball);

			// Respawn pocisku w pozycji i rotacji punktu spawn
			GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

			// Ustaw cel dla pocisku, zak³adaj¹c ¿e prefab ma komponent 'Projectile'
			Projectile projectileScript = projectile.GetComponent<Projectile>();
			if (projectileScript != null)
			{
				// Ustaw cel dla pocisku
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
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
