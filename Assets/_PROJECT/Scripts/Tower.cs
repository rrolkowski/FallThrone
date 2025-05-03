using UnityEngine;

public class Tower : MonoBehaviour
{
	public GameObject projectilePrefab;  // Prefab pocisku
	public Transform spawnPoint;         // Punkt, z którego s¹ spawnowane pociski
	public float detectionRange = 10f;   // Zasiêg wykrywania wrogów
	public float fireRate = 1f;          // Czêstotliwoœæ strza³ów (czas w sekundach miêdzy kolejnymi strza³ami)
	private float nextFireTime = 0f;     // Czas do nastêpnego strza³u

    [SerializeField] private TargetingStrategy targetingStrategy = TargetingStrategy.Closest;


    // Update is called once per frame
    void Update()
	{
		// Sprawdzenie, czy mo¿na ju¿ wystrzeliæ
		if (Time.time >= nextFireTime)
		{
            // Szukaj najbli¿szego wroga w zasiêgu
            GameObject targetEnemy = FindTargetEnemy();
            if (targetEnemy != null)
            {
                Shoot(targetEnemy);
                nextFireTime = Time.time + fireRate;
            }

        }
    }

    // Funkcja wyszukuj¹ca najbli¿szego wroga w zasiêgu
    GameObject FindTargetEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject selectedEnemy = null;
        float bestValue = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance > detectionRange) continue;

            switch (targetingStrategy)
            {
                case TargetingStrategy.Closest:
                    if (distance < bestValue)
                    {
                        bestValue = distance;
                        selectedEnemy = enemy;
                    }
                    break;

                case TargetingStrategy.LowestHP:
                    if (enemy.TryGetComponent(out HealthController hp))
                    {
                        if (hp.currentHealth < bestValue)
                        {
                            bestValue = hp.currentHealth;
                            selectedEnemy = enemy;
                        }
                    }
                    break;
            }
        }

        return selectedEnemy;
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

    public enum TargetingStrategy
    {
        Closest,
        LowestHP
    }

}
