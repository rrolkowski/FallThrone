using System.Collections;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public static PauseMenu Instance;

	public GameObject pauseMenuCanvas; // Canvas Pause Menu

	private void Awake()
	{
		Instance = this;
	}

	void Update()
	{
		if (GameState.STATE_LoadingLevel) return;

		// Sprawdzanie, czy gracz nacisn¹³ ESC
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			var gameController = GameController.Instance;
			if (GameState.STATE_Lost || GameState.STATE_Won)
			{
				return;
			}

			if (GameState.STATE_Shop)
			{
				// Zamknij sklep, jeœli jest otwarty
				ShopManager.Instance.ToggleShop();
				return;
			}

			if (GameState.STATE_Paused)
			{
				ResumeGame();
			}
			else
			{
				PauseGame();
			}
		}
	}

	// Wznawianie gry
	public void ResumeGame()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);

		pauseMenuCanvas.SetActive(false); // Ukryj menu pauzy
		Time.timeScale = 1f; // Przywróæ normalny czas gry
		GameState.STATE_Paused = false; // Ustaw flagê
	}

	// Wstrzymanie gry
	public void PauseGame()
	{
		pauseMenuCanvas.SetActive(true); // Poka¿ menu pauzy
		Time.timeScale = 0f; // Zatrzymaj czas gry
		GameState.STATE_Paused = true; // Ustaw flagê
	}

	// Restart sceny
	public void RestartLevel()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);
		StartCoroutine(RestartAfterFadeOut());
	}

	private IEnumerator RestartAfterFadeOut()
	{
		ScreenFader.Instance.FadeOut(); // Uruchomienie animacji fade-out
		yield return new WaitForSecondsRealtime(1); // Poczekaj na zakoñczenie animacji (1 sekunda)

		Time.timeScale = 1f; // Przywróæ normalny czas gry przed restartem
		SceneManagerScript.Instance.ReloadCurrentScene();
	}

	// Wyjœcie do g³ównego menu
	public void ExitToMenu()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);

		Time.timeScale = 1f; // Przywróæ normalny czas gry przed wyjœciem
		SceneManagerScript.Instance.LoadMenuScene();
	}
}
