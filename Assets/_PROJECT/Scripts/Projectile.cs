using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed = 5f; // Prêdkoœæ poruszania siê pocisku
	private Transform target; // Cel - wróg
	private Vector3 lastKnownPosition; // Ostatnia znana pozycja celu
	private bool targetLost = false; // Flaga sprawdzaj¹ca, czy cel zosta³ utracony
	public float damage = 10f; // Iloœæ zadawanych obra¿eñ

	// Update is called once per frame
	void Update()
	{
		if (target != null)
		{
			// Ruch pocisku w stronê celu
			Vector3 direction = (target.position - transform.position).normalized;
			transform.position += direction * speed * Time.deltaTime;

			// Aktualizacja ostatniej znanej pozycji wroga
			lastKnownPosition = target.position;

			// Obrót pocisku w kierunku przeciwnika
			transform.LookAt(target);
		}
		else if (targetLost == false) // Jeœli cel zosta³ utracony
		{
			targetLost = true; // Flaga, ¿e cel zosta³ zgubiony

			// Celujemy w ostatni¹ znan¹ pozycjê
			Vector3 direction = (lastKnownPosition - transform.position).normalized;
			transform.position += direction * speed * Time.deltaTime;

			// Obracanie siê w kierunku ostatniej znanej pozycji
			transform.LookAt(lastKnownPosition);
		}
		else
		{
			// Gdy cel zostanie zgubiony, poruszaj siê w stronê ostatniej znanej pozycji
			if (Vector3.Distance(transform.position, lastKnownPosition) > 0.1f)
			{
				Vector3 direction = (lastKnownPosition - transform.position).normalized;
				transform.position += direction * speed * Time.deltaTime;
			}
			else
			{
				// Jeœli dotarliœmy do ostatniej znanej pozycji, mo¿emy zniszczyæ pocisk
				Destroy(gameObject);
			}
		}
	}

	// Ustawienie celu pocisku
	public void SetTarget(Transform enemy)
	{
		target = enemy;
	}

	// Funkcja wywo³ywana przy kolizji z innym obiektem
	private void OnTriggerEnter(Collider other)
	{
		// Sprawdzamy, czy obiekt, z którym kolidujemy, ma skrypt HealthController
		HealthController healthController = other.GetComponent<HealthController>();

		// Jeœli obiekt posiada skrypt i jego typ to Enemy, zadajemy mu obra¿enia
		if (healthController != null && healthController.objectType == HealthController.ObjectType.Enemy)
		{
			healthController.TakeDamage(damage); // Zadajemy obra¿enia
			Destroy(gameObject); // Zniszczenie pocisku po kolizji
		}
	}
}
