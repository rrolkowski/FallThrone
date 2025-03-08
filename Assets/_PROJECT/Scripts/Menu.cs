using System.Collections;
using UnityEngine;

public class Menu : MonoBehaviour
{
	public GameObject menuCanvas; // Canvas g��wnego menu
	public GameObject creditsCanvas; // Canvas credits
	public GameObject guideCanvas; // Canvas guide
	public GameObject levelSelectCanvas; // Canvas level select

	private void Start()
	{
		// Upewnij si�, �e tylko menu jest aktywne na pocz�tku
		menuCanvas.SetActive(true);
		creditsCanvas.SetActive(false);
		guideCanvas.SetActive(false);
		levelSelectCanvas.SetActive(false);
	}

	// Metoda przypisana do przycisku Start
	public void StartGame(int loadLevel)
	{
		// Uruchamiamy coroutine, aby wykona� fade-out i zmieni� scen� po zako�czeniu
		StartCoroutine(StartGameWithFade(loadLevel));
	}

	private IEnumerator StartGameWithFade(int level)
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Play_Button);

		// Wywo�aj fade-out (czarny ekran)
		ScreenFader.Instance.FadeOut();

		// Poczekaj, a� fade-out zostanie zako�czony
		yield return new WaitForSecondsRealtime(ScreenFader.Instance.fadeDuration);

		// Po fade-out za�aduj now� scen�
		SceneManagerScript.Instance.LoadLevel(level);
	}

	// Metoda przypisana do przycisku Level Select
	public void ShowLevelSelect()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);

		// Dezaktywacja menu i aktywacja level select
		menuCanvas.SetActive(false);
		levelSelectCanvas.SetActive(true);
	}

	// Metoda przypisana do przycisku Credits
	public void ShowCredits()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);

		// Dezaktywacja menu i aktywacja credits
		menuCanvas.SetActive(false);
		creditsCanvas.SetActive(true);
	}

	// Metoda przypisana do przycisku Exit
	public void ExitGame()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);

		// Dzia�a zar�wno w buildzie, jak i w edytorze Unity
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	// Metoda przypisana do przycisku Go Back
	public void GoBack()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);

		// Dezaktywacja innych canvas�w i powr�t do menu g��wnego
		creditsCanvas.SetActive(false);
		guideCanvas.SetActive(false);
		levelSelectCanvas.SetActive(false);
		menuCanvas.SetActive(true);
	}

	// Metoda przypisana do przycisku Guide
	public void ShowGuide()
	{
		AudioManager.PlaySound(SoundType.MENU_Select_Button);

		// Dezaktywacja menu i aktywacja guide
		menuCanvas.SetActive(false);
		guideCanvas.SetActive(true);
	}
}
