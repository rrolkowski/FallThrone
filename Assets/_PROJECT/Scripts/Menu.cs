using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
	public GameObject menuCanvas;
	public GameObject creditsCanvas;
	public GameObject guideCanvas;
	public GameObject levelSelectCanvas;

	[SerializeField] private Button[] levelButtons;

	private int unlockedLevel = 0;

	// Obiekt 3D do animacji
	[SerializeField] private Transform animatedObject;
	private Vector3 originalPosition;
	private Vector3 targetPosition;

	[SerializeField] private Transform blockInteraction_Object;


	private void Start()
	{
		InitializeProgress();
		SetupCanvases();
		UpdateLevelButtons();

		// Zapamiętaj pozycję oryginalną
		if (animatedObject != null)
		{
			originalPosition = animatedObject.position;
			targetPosition = originalPosition;
		}
	}

	private void InitializeProgress()
	{
		const int defaultUnlockedLevel = 0;

		if (!PlayerPrefs.HasKey("UnlockedLevel") || PlayerPrefs.GetInt("UnlockedLevel") < defaultUnlockedLevel)
		{
			PlayerPrefs.SetInt("UnlockedLevel", defaultUnlockedLevel);
			PlayerPrefs.Save();
		}

		unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", defaultUnlockedLevel);
	}

	private void SetupCanvases()
	{
		menuCanvas.SetActive(true);
		creditsCanvas.SetActive(false);
		guideCanvas.SetActive(false);
		levelSelectCanvas.SetActive(false);
	}

	private void UpdateLevelButtons()
	{
		for (int i = 0; i < levelButtons.Length; i++)
		{
			bool isUnlocked = i <= unlockedLevel;

			levelButtons[i].interactable = isUnlocked;

			Transform container = levelButtons[i].transform.Find("Container");
			if (container != null)
			{
				SetVisualOpacity(container.gameObject, isUnlocked ? 1f : 0.1f);
			}

			ButtonHighlightEffect highlightEffect = levelButtons[i].GetComponent<ButtonHighlightEffect>();
			if (highlightEffect != null)
			{
				highlightEffect.enabled = isUnlocked;
			}
		}
	}

	private void SetVisualOpacity(GameObject root, float alpha)
	{
		// Obsługa Image (UI)
		Image[] images = root.GetComponentsInChildren<Image>(true);
		foreach (var image in images)
		{
			Color c = image.color;
			c.a = alpha;
			image.color = c;
		}

		// Obsługa TextMeshProUGUI
		TMPro.TextMeshProUGUI[] texts = root.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
		foreach (var text in texts)
		{
			Color c = text.color;
			c.a = alpha;
			text.color = c;
		}
	}

	public void TryStartGame(int levelIndex)
	{
		if (levelIndex <= unlockedLevel)
		{
			StartGame(levelIndex);
			blockInteraction_Object.gameObject.SetActive(true);
		}
		else
		{
			Debug.LogWarning($"[TRY START GAME] Level {levelIndex} jest zablokowany. Max odblokowany: {unlockedLevel}");
		}
	}

	public void StartGame(int loadLevel)
	{
		StartCoroutine(StartGameWithFade(loadLevel));
	}

	private IEnumerator StartGameWithFade(int level)
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Play_Button);
		ScreenFader.Instance.FadeOut();
		yield return new WaitForSecondsRealtime(ScreenFader.Instance.fadeDuration);
		SceneManagerScript.Instance.LoadLevel(level);
	}

	public void ShowLevelSelect()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);
		menuCanvas.SetActive(false);
		levelSelectCanvas.SetActive(true);

		if (animatedObject != null)
		{
			targetPosition = originalPosition + new Vector3(-3f, 0f, 0f);
		}
	}

	public void ShowCredits()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);
		menuCanvas.SetActive(false);
		creditsCanvas.SetActive(true);

		if (animatedObject != null)
		{
			targetPosition = originalPosition + new Vector3(-3f, 0f, 0f);
		}
	}

	public void ShowGuide()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);
		menuCanvas.SetActive(false);
		guideCanvas.SetActive(true);

		if (animatedObject != null)
		{
			targetPosition = originalPosition + new Vector3(-3f, 0f, 0f);
		}
	}

	public void GoBack()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);
		creditsCanvas.SetActive(false);
		guideCanvas.SetActive(false);
		levelSelectCanvas.SetActive(false);
		menuCanvas.SetActive(true);

		if (animatedObject != null)
		{
			targetPosition = originalPosition;
		}
	}

	public void ExitGame()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	// 🔁 Reset Progress z przycisku
	public void ResetProgressButton()
	{
		PlayerPrefs.SetInt("UnlockedLevel", 0);
		unlockedLevel = 0;

		// Dodatkowo resetujemy gwiazdki dla wszystkich leveli 1..9
		for (int level = 1; level <= 9; level++)
		{
			PlayerPrefs.DeleteKey($"Level_{level}_Stars");
		}

		PlayerPrefs.Save();

		UpdateLevelButtons();
		UpdateLevelButtons(); // tak - musi byc 2 razy bo jak jest raz to nie działa pozdrawiam

		// 🔄 Force update stars display
		LevelStarsDisplay starsDisplay = FindObjectOfType<LevelStarsDisplay>();
		if (starsDisplay != null)
		{
			starsDisplay.ForceUpdateStars();
		}
	}



	// 🔓 Unlock All z przycisku
	public void UnlockAllLevelsButton()
	{
		unlockedLevel = levelButtons.Length - 1;
		PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel);
		PlayerPrefs.Save();

		UpdateLevelButtons();
		Debug.Log("[UNLOCK] Wszystkie poziomy zostały odblokowane.");
	}

	private void Update()
	{
		if (animatedObject != null)
		{
			// Płynne przejście w stronę targetPosition
			animatedObject.position = Vector3.Lerp(animatedObject.position, targetPosition, Time.deltaTime * 3f); // 5f = prędkość płynności
		}
	}
}
