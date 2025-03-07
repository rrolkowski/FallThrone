using UnityEngine;

public class Tower : MonoBehaviour
{
	public GameObject projectilePrefab;  // Prefab pocisku
	public Transform spawnPoint;         // Punkt, z kt�rego s� spawnowane pociski
	public float detectionRange = 10f;   // Zasi�g wykrywania wrog�w
	public float fireRate = 1f;          // Cz�stotliwo�� strza��w (czas w sekundach mi�dzy kolejnymi strza�ami)
	private float nextFireTime = 0f;     // Czas do nast�pnego strza�u

	// Update is called once per frame
	void Update()
	{
		// Sprawdzenie, czy mo�na ju� wystrzeli�
		if (Time.time >= nextFireTime)
		{
			// Szukaj najbli�szego wroga w zasi�gu
			GameObject nearestEnemy = FindClosestEnemy();
			if (nearestEnemy != null)
			{
				// Respawn pocisku
				Shoot(nearestEnemy);
				// Ustaw czas na nast�pny strza�
				nextFireTime = Time.time + fireRate;
			}
		}
	}

	// Funkcja wyszukuj�ca najbli�szego wroga w zasi�gu
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

	// Funkcja strzelaj�ca w kierunku wroga
	void Shoot(GameObject target)
	{
		if (spawnPoint != null)
		{
			//AUDIO
			AudioManager.PlaySound(SoundType.GAME_Turret_Fireball);

			// Respawn pocisku w pozycji i rotacji punktu spawn
			GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

			// Ustaw cel dla pocisku, zak�adaj�c �e prefab ma komponent 'Projectile'
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
