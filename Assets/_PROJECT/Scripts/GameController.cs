using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using System.Collections;

public class GameController : MonoBehaviour
{
	public static GameController Instance;

    public event Action OnPickedUp;

	//Star rating
    [HideInInspector] public int starRating = 0;

    // Health system
    [Header("Health Settings")]
	public int maxHealth = 3;
	public int currentHealth = 3;
	public Transform healthContainer;
	public GameObject heartPrefab;
	private List<Image> hearts = new List<Image>();

	// Tower system
	[Header("Tower Settings")]
	public int maxTower_Points = 5;
	public int currentTower_Points = 0;
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
    public Image[] starIcons;
    public Sprite activeStarSprite;
    public Sprite inactiveStarSprite;

    //POWER-UP
    [Header("Power-Up UI")]
    [SerializeField] TextMeshProUGUI _powerUpText;
    [SerializeField] private Image _powerUpImage;
    [SerializeField] private Sprite _defaultPowerUpSprite;
    private BasePowerUp _currentPowerUp;
    private float _powerUpTimeRemaining = 0f;
    private float _powerUpDuration = 1f;

	[Header("Enemy Defeat Counter")]
	public int DefeatedEnemies = 0;

	private bool localwincondition;

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

    void Update()
    {
        if (!GameState.STATE_Won)
            GameController.Instance.CheckForWinCondition();
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
	public void AddTower(int tower_cost)
	{
		if (currentTower_Points < maxTower_Points)
		{
			currentTower_Points = currentTower_Points + tower_cost;
			UpdateTowerText();
		}
	}

	public void RemoveTower(int tower_cost)
	{
		if (currentTower_Points > 0)
		{
			currentTower_Points = currentTower_Points - tower_cost;
			UpdateTowerText();
		}
	}

	private void UpdateTowerText()
	{
		towerText.text = $"{currentTower_Points}/{maxTower_Points}";
	}

	public void SetMaxTowers(int newMaxTowers)
	{
		maxTower_Points = newMaxTowers;

		// Ensure currentTower_Points is not greater than maxTower_Points
		if (currentTower_Points > maxTower_Points)
		{
			currentTower_Points = maxTower_Points;
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

    #region Star Rating
    public void StarRating()
    {
        for (int i = 0; i < starIcons.Length; i++)
        {
            starIcons[i].sprite = i < starRating ? activeStarSprite : inactiveStarSprite;
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

        starRating = Mathf.Clamp(currentHealth, 0, 3);
		StarRating();
		Debug.Log("Stars" + starRating);

		SceneManagerScript.Instance.UnlockNextLevel();

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
		if (!GameState.STATE_Won && CheckWinCondition() && !localwincondition)
		{
			GameWin();
			localwincondition = true;
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

    #region PowerUp

    private string FormatPowerUpName(string name)
    {
        return System.Text.RegularExpressions.Regex.Replace(name, "(\\B[A-Z])", " $1");
    }

    public void SetPowerUp(BasePowerUp powerUp)
    {
        _currentPowerUp = powerUp;
        _powerUpText.text = FormatPowerUpName(powerUp.GetType().Name);

        SpriteRenderer spriteRenderer = powerUp.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            _powerUpImage.sprite = spriteRenderer.sprite;
            _powerUpImage.enabled = true;
        }
        else
        {
            _powerUpImage.sprite = _defaultPowerUpSprite;
        }
    }
    public void StartPowerUpTimer(float duration)
    {
        _powerUpDuration = duration;
        _powerUpTimeRemaining = duration;
        _powerUpImage.fillAmount = 1f;

        StartCoroutine(UpdatePowerUpTimer());
    }

    private IEnumerator UpdatePowerUpTimer()
    {
        while (_powerUpTimeRemaining > 0)
        {
            _powerUpTimeRemaining -= Time.deltaTime;
            _powerUpImage.fillAmount = _powerUpTimeRemaining / _powerUpDuration;

            yield return null;
        }

        _powerUpImage.fillAmount = 0f;
        OnPickedUp?.Invoke();
    }

    public void ClearPowerUp()
    {
        _currentPowerUp = null;
        _powerUpText.text = "";
        _powerUpImage.sprite = _defaultPowerUpSprite;
        _powerUpImage.fillAmount = 1f;
    }

    public void ActivatePowerUp()
    {
        if (_currentPowerUp != null)
        {
            _currentPowerUp.ApplyEffect();
			Debug.Log("Apply Effect (GC)");
            _currentPowerUp = null;
        }
    }
	#endregion

	public void EnemyDefeated()
	{
		DefeatedEnemies++;
		CheckForWinCondition();
	}

}
