using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
	public EnemyMovement enemymovement;

	private System.Action<Unit> _returnToPool;

	public enum ObjectType
	{
		NotAssigned,
		Player,
		Enemy,
		Tower,
		Core
	}

	public ObjectType objectType;
	public float maxHealth = 100f;
	public float currentHealth;

	[SerializeField] private Slider healthSlider;
	[SerializeField] private ParticleSystem hitParticleEffectNormal;
	[SerializeField] private ParticleSystem hitParticleEffectIce;

	public void Init(System.Action<Unit> returnToPool)
	{
		_returnToPool = returnToPool;
	}

	void Start()
	{
		currentHealth = maxHealth;

		if (healthSlider != null)
		{
			healthSlider.maxValue = maxHealth;
			healthSlider.value = currentHealth;
			healthSlider.gameObject.SetActive(currentHealth < maxHealth);
		}
	}

	void Update()
	{
		if (currentHealth <= 0)
		{
			Die();
		}

		if (healthSlider != null)
		{
			healthSlider.value = currentHealth;
			healthSlider.gameObject.SetActive(currentHealth < maxHealth);
		}
	}

	public void TakeDamage(float damage, string towerType)
	{
		// 🚫 Jeśli gra wygrana lub przegrana – ignoruj
		if (GameState.STATE_Won || GameState.STATE_Lost)
			return;

		if (TryGetComponent<ShieldEnemy>(out var shieldEnemy))
		{
			if (shieldEnemy.IsShieldActive)
				return;
		}

		currentHealth -= damage;

		if (enemymovement.enemyType == EnemyType.Bomba)
		{
			switch (towerType)
			{
				case "normal":
					AudioManager.PlaySound(SoundType.GAME_Enemy_Hit);
					SpawnEffect(hitParticleEffectNormal);
					break;

				case "ice":
					AudioManager.PlaySound(SoundType.GAME_Enemy_Hit_IceTower);
					SpawnEffect(hitParticleEffectIce);
					break;

				case "aoe":
					AudioManager.PlaySound(SoundType.GAME_Enemy_Hit_AOETower);
					break;
			}
		}

		if (enemymovement.enemyType == EnemyType.Bober)
		{
			AudioManager.PlaySound(SoundType.GAME_Enemy_Bober_Hit);
			SpawnEffect(hitParticleEffectNormal);
		}

		if (enemymovement.enemyType == EnemyType.Duch)
		{
			AudioManager.PlaySound(SoundType.GAME_Enemy_Duch_Hit);
			SpawnEffect(hitParticleEffectNormal);
		}

		if (enemymovement.enemyType == EnemyType.Slimak)
		{
			AudioManager.PlaySound(SoundType.GAME_Enemy_Slimak_Hit);
			SpawnEffect(hitParticleEffectNormal);
		}

		if (healthSlider != null)
		{
			healthSlider.value = currentHealth;
			healthSlider.gameObject.SetActive(currentHealth < maxHealth);
		}

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	private void SpawnEffect(ParticleSystem effectPrefab)
	{
		if (effectPrefab == null) return;

		var effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
		effect.Play();
		Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
	}

	public void ResetHealth()
	{
		currentHealth = maxHealth;

		if (healthSlider != null)
		{
			healthSlider.value = currentHealth;
			healthSlider.gameObject.SetActive(currentHealth < maxHealth);
		}

		Debug.Log($"Reset health for {name}. Current health: {currentHealth}");
	}

	void Die()
	{
		switch (objectType)
		{
			case ObjectType.Player:
				Destroy(gameObject);
				break;

			case ObjectType.Enemy:
				AudioManager.PlaySound(SoundType.GAME_Enemy_Death);
				GameController.Instance.AddPoints(25);
                ObjectGrabber.Instance?._rangeCircleController.DeactivateRangeCircle();
                if (ObjectGrabber.Instance.currentlyGrabbedObject == gameObject)
                {
                    ObjectGrabber.Instance.currentlyGrabbedObject = null;
                    transform.SetParent(null);
                }

                if (TryGetComponent<Unit>(out var unit))
				{
					Debug.Log("Invoking ReturnToPool for: " + unit.name);
					_returnToPool?.Invoke(unit);
				}
				else
				{
					Debug.LogWarning("Unit component not found, destroying object.");
					Destroy(gameObject);
				}
				break;

			case ObjectType.Tower:
			case ObjectType.Core:
				Destroy(gameObject);
				break;
		}
	}
}
