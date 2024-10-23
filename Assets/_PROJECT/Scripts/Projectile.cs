using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed = 5f; // Pr�dko�� poruszania si� pocisku
	private Transform target; // Cel - wr�g
	private Vector3 lastKnownPosition; // Ostatnia znana pozycja celu
	private bool targetLost = false; // Flaga sprawdzaj�ca, czy cel zosta� utracony
	public float damage = 10f; // Ilo�� zadawanych obra�e�

	// Update is called once per frame
	void Update()
	{
		if (target != null)
		{
			// Ruch pocisku w stron� celu
			Vector3 direction = (target.position - transform.position).normalized;
			transform.position += direction * speed * Time.deltaTime;

			// Aktualizacja ostatniej znanej pozycji wroga
			lastKnownPosition = target.position;

			// Obr�t pocisku w kierunku przeciwnika
			transform.LookAt(target);
		}
		else if (targetLost == false) // Je�li cel zosta� utracony
		{
			targetLost = true; // Flaga, �e cel zosta� zgubiony

			// Celujemy w ostatni� znan� pozycj�
			Vector3 direction = (lastKnownPosition - transform.position).normalized;
			transform.position += direction * speed * Time.deltaTime;

			// Obracanie si� w kierunku ostatniej znanej pozycji
			transform.LookAt(lastKnownPosition);
		}
		else
		{
			// Gdy cel zostanie zgubiony, poruszaj si� w stron� ostatniej znanej pozycji
			if (Vector3.Distance(transform.position, lastKnownPosition) > 0.1f)
			{
				Vector3 direction = (lastKnownPosition - transform.position).normalized;
				transform.position += direction * speed * Time.deltaTime;
			}
			else
			{
				// Je�li dotarli�my do ostatniej znanej pozycji, mo�emy zniszczy� pocisk
				Destroy(gameObject);
			}
		}
	}

	// Ustawienie celu pocisku
	public void SetTarget(Transform enemy)
	{
		target = enemy;
	}

	// Funkcja wywo�ywana przy kolizji z innym obiektem
	private void OnTriggerEnter(Collider other)
	{
		// Sprawdzamy, czy obiekt, z kt�rym kolidujemy, ma skrypt HealthController
		HealthController healthController = other.GetComponent<HealthController>();

		// Je�li obiekt posiada skrypt i jego typ to Enemy, zadajemy mu obra�enia
		if (healthController != null && healthController.objectType == HealthController.ObjectType.Enemy)
		{
			healthController.TakeDamage(damage); // Zadajemy obra�enia
			Destroy(gameObject); // Zniszczenie pocisku po kolizji
		}
	}
}
