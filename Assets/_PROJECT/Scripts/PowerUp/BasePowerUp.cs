using System;
using UnityEngine;

public abstract class BasePowerUp : MonoBehaviour
{
	public GameObject pickupEffectPrefab;

	public abstract void ApplyEffect();
	protected abstract SoundType GetPickupSound();

	private void Update()
	{
		Debug.Log("xd");
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			Instantiate(pickupEffectPrefab, transform.position, Quaternion.Euler(90, 0, 0));

			AudioManager.PlaySound(GetPickupSound());

			GameController.Instance.SetPowerUp(this);

			// Wy³¹czenie wszystkich Colliderów w obiekcie i jego dzieciach
			foreach (var col in GetComponentsInChildren<Collider>())
				col.enabled = false;

			// Usuniêcie wszystkich dzieci obiektu
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}

		}
	}
}
