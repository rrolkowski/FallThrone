using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class GameController : MonoBehaviour
{
	public static GameController Instance;

	// Health system
	[Header("Health Settings")]
	public int maxHealth = 3;
	public int currentHealth = 3;
	public Transform healthContainer;
	public GameObject heartPrefab;
	private List<Image> hearts = new List<Image>();

	// Tower system
	[Header("Tower Settings")]
	public int maxTowers = 3;
	public int currentTowers = 0;
	public TextMeshProUGUI towerText;

	// Economy system
	[Header("Economy Settings")]
	public int points = 0;
	public TextMeshProUGUI pointsText;

	// Lost Canvas
	[Header("Lost Canvas")]
	public GameObject lostCanvas;

	// Win condition
	[Header("Win Canvas")]
	public GameObject winCanvas;

	private void Awake()
	{
		// Singleton pattern
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		// Initialize systems
		InitializeHealthUI();
		UpdateTowerText();
		UpdatePointsText();

		// Ensure canvases are disabled at start
		if (lostCanvas != null)
		{
			lostCanvas.SetActive(false);
		}
		if (winCanvas != null)
		{
			winCanvas.SetActive(false);
		}
	}

	private void Start()
	{
		if (ScreenFader.Instance != null)
		{
			StartCoroutine(Fade());
		}
	}

	private IEnumerator Fade()
	{
		yield return new WaitForSecondsRealtime(1);
		ScreenFader.Instance.FadeIn();
	}

	#region Health System
	public void TakeDamage()
	{
		if (currentHealth > 0)
		{
			AudioManager.PlaySound(SoundType.GAME_Health);
			currentHealth--;
			UpdateHealthUI();
			Debug.Log($"Health: {currentHealth}");

			// Check for GameOver condition
			if (!GameState.STATE_Lost && currentHealth <= 0)
			{
				GameOver();
			}
		}
	}

	private void InitializeHealthUI()
	{
		// Clear existing hearts
		foreach (Transform child in healthContainer)
		{
			Destroy(child.gameObject);
		}
		hearts.Clear();

		// Create hearts based on maxHealth
		for (int i = 0; i < maxHealth; i++)
		{
			GameObject heart = Instantiate(heartPrefab, healthContainer);
			Image heartImage = heart.GetComponent<Image>();
			if (heartImage != null)
			{
				hearts.Add(heartImage);
			}
		}
		UpdateHealthUI();
	}

	private void UpdateHealthUI()
	{
		for (int i = 0; i < hearts.Count; i++)
		{
			Color color = hearts[i].color;
			color.a = (i < currentHealth) ? 1f : 0.1f;
			hearts[i].color = color;
		}
	}
	#endregion

	#region Tower System
	public void AddTower()
	{
		if (currentTowers < maxTowers)
		{
			currentTowers++;
			UpdateTowerText();
		}
	}

	public void RemoveTower()
	{
		if (currentTowers > 0)
		{
			currentTowers--;
			UpdateTowerText();
		}
	}

	private void UpdateTowerText()
	{
		towerText.text = $"{currentTowers}/{maxTowers}";
	}

	public void SetMaxTowers(int newMaxTowers)
	{
		maxTowers = newMaxTowers;

		// Ensure currentTowers is not greater than maxTowers
		if (currentTowers > maxTowers)
		{
			currentTowers = maxTowers;
		}

		UpdateTowerText();
	}
	#endregion

	#region Economy System
	public void AddPoints(int amount)
	{
		points = Mathf.Min(999, points + amount); // Limit maximum points to 999
		UpdatePointsText();
	}

	public void RemovePoints(int amount)
	{
		points = Mathf.Max(0, points - amount); // Prevent negative points
		UpdatePointsText();
	}

	private void UpdatePointsText()
	{
		if (pointsText != null)
		{
			pointsText.text = points.ToString("D3"); // Format with three digits
		}
	}
	#endregion

	#region Game Control
	private void GameOver()
	{
		MusicManager.StopMusic();

		GameState.STATE_Lost = true;
		if (lostCanvas != null)
		{
			ShopManager.Instance.GUI_Shop.SetActive(false);
			PauseMenu.Instance.pauseMenuCanvas.SetActive(false);

			lostCanvas.SetActive(true);
		}

		AudioManager.PlaySound(SoundType.GAME_Lost);
		Debug.Log("Health reached 0. Game Over.");
		Time.timeScale = 0f;
	}

	private void GameWin()
	{
		MusicManager.StopMusic();

		GameState.STATE_Won = true;
		if (winCanvas != null)
		{
			ShopManager.Instance.GUI_Shop.SetActive(false);
			PauseMenu.Instance.pauseMenuCanvas.SetActive(false);

			winCanvas.SetActive(true);
		}

		AudioManager.PlaySound(SoundType.GAME_Win);
		Debug.Log("Win condition met. Game Won!");
		Time.timeScale = 0f;
	}

	public void CheckForWinCondition()
	{
		if (!GameState.STATE_Won && CheckWinCondition())
		{
			GameWin();
		}
	}

	private bool CheckWinCondition()
	{
		// Pobierz wszystkie obiekty w scenie z tagiem "Enemy"
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

		// SprawdŸ, czy wszystkie s¹ nieaktywne
		foreach (var enemy in enemies)
		{
			if (enemy.activeSelf) // Jeœli choæ jeden wróg jest aktywny
			{
				return false; // Gra jeszcze nie jest wygrana
			}
		}

		// Jeœli ¿aden wróg nie jest aktywny i liczba pokonanych wrogów równa maksymalnej liczbie
		return UnitSpawner.Instance._spawnedUnits >= UnitSpawner.Instance._maxEnemyUnits;
	}
	#endregion
}
