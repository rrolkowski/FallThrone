using UnityEngine;

public class Projectile : MonoBehaviour
{
	public enum ProjectileType { Fireball, Ice }
	public ProjectileType projectileType;

	public float speed = 10f;
	private Transform target;
	private Vector3 lastKnownPosition;
	private bool targetLost = false;
	public float damage = 10f;

	public float slowDuration = 2f; // Czas trwania spowolnienia dla lodowego pocisku
	public float slowPercentage = 0.5f; // O ile % zmniejszamy prêdkoœæ (np. 0.5 = 50%)

	void Update()
	{
		if (target != null && target.gameObject.activeInHierarchy)
		{
			Vector3 direction = (target.position - transform.position).normalized;
			transform.position += direction * speed * Time.deltaTime;
			lastKnownPosition = target.position;
			transform.LookAt(target);
		}
		else if (!targetLost)
		{
			targetLost = true;
			Vector3 direction = (lastKnownPosition - transform.position).normalized;
			transform.position += direction * speed * Time.deltaTime;
			transform.LookAt(lastKnownPosition);
		}
		else
		{
			if (Vector3.Distance(transform.position, lastKnownPosition) > 0.1f)
			{
				Vector3 direction = (lastKnownPosition - transform.position).normalized;
				transform.position += direction * speed * Time.deltaTime;
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	public void SetTarget(Transform enemy)
	{
		target = enemy;
	}

	private void OnTriggerEnter(Collider other)
	{
		HealthController healthController = other.GetComponent<HealthController>();
		EnemyMovement enemyMovement = other.GetComponent<EnemyMovement>(); // Pobieramy skrypt ruchu przeciwnika

		if (healthController != null && healthController.objectType == HealthController.ObjectType.Enemy && other.gameObject.activeInHierarchy)
		{
			ApplyProjectileEffect(healthController, enemyMovement);
			Destroy(gameObject);
		}
	}

	private void ApplyProjectileEffect(HealthController healthController, EnemyMovement enemyMovement)
	{
		switch (projectileType)
		{
			case ProjectileType.Fireball:
				healthController.TakeDamage(damage);
				break;

			case ProjectileType.Ice:
				healthController.TakeDamage(damage);
				if (enemyMovement != null)
				{
					enemyMovement.ApplySlow(slowDuration, slowPercentage);
				}
				break;
		}
	}
}
