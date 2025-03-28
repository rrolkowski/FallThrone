using System;
using UnityEngine;

public abstract class BasePowerUp : MonoBehaviour
{
	public abstract void ApplyEffect();
	protected abstract SoundType GetPickupSound();

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
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
