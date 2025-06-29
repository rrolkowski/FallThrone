using System.Collections;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public static PauseMenu Instance;

	public GameObject pauseMenuCanvas;

	private void Awake()
	{
		Instance = this;
	}

	void Update()
	{
		if (GameState.STATE_LoadingLevel) return;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			var gameController = GameController.Instance;

			// Blokada menu pauzy w trakcie tutoriala
			if (gameController != null && gameController.isTutorialActive)
			{
				Debug.Log("Cannot pause the game during tutorial!");
				return;
			}

			if (GameState.STATE_Lost || GameState.STATE_Won)
			{
				return;
			}

			if (GameState.STATE_Shop)
			{
				ShopManager.Instance.ToggleShop();
				return;
			}

			if (GameState.STATE_Paused)
			{
				ResumeGameNoAudio();
			}
			else
			{
				PauseGame();
			}
		}
	}

	public void ResumeGame()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);
		pauseMenuCanvas.SetActive(false);
		Time.timeScale = 1f;
		GameState.STATE_Paused = false;
	}

	public void ResumeGameNoAudio()
	{
		pauseMenuCanvas.SetActive(false);
		Time.timeScale = 1f;
		GameState.STATE_Paused = false;
	}

	public void PauseGame()
	{
		pauseMenuCanvas.SetActive(true);
		Time.timeScale = 0f;
		GameState.STATE_Paused = true;
	}

	public void RestartLevel()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);
		StartCoroutine(RestartAfterFadeOut());
	}

	private IEnumerator RestartAfterFadeOut()
	{
		ScreenFader.Instance.FadeOut();
		yield return new WaitForSecondsRealtime(1);
		Time.timeScale = 1f;
		SceneManagerScript.Instance.ReloadCurrentScene();
	}

	public void ExitToMenu()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);
		Time.timeScale = 1f;
		SceneManagerScript.Instance.LoadMenuScene();
	}

	public void LoadNextLevel()
	{
		AudioManager.PlaySound(SoundType.MENU_NextLevel_Button);
		StartCoroutine(LoadNextLevelAfterFadeOut());
	}

	private IEnumerator LoadNextLevelAfterFadeOut()
	{
		ScreenFader.Instance.FadeOut();
		yield return new WaitForSecondsRealtime(1);
		Time.timeScale = 1f;
		SceneManagerScript.Instance.LoadNextLevel();
	}
}
