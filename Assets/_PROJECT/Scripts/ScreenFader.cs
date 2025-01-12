using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
	public static ScreenFader Instance;

	public Image blackScreenImage; // Obraz czarnego ekranu (ustawi� w Inspectorze)
	public float fadeDuration = 1.0f; // Czas trwania przej�cia (w sekundach)

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Publiczna funkcja do wywo�ania fade in (czarny ekran -> przezroczysto��)
	public void FadeIn()
	{
		StartCoroutine(Fade(1, 0));
	}

	// Publiczna funkcja do wywo�ania fade out (przezroczysto�� -> czarny ekran)
	public void FadeOut()
	{
		StartCoroutine(Fade(0, 1));
	}

	// Funkcja odpowiedzialna za animacj� alpha
	private IEnumerator Fade(float startAlpha, float endAlpha)
	{
		float elapsedTime = 0f;
		Color screenColor = blackScreenImage.color;

		// Ustawienie pocz�tkowej warto�ci alpha
		screenColor.a = startAlpha;
		blackScreenImage.color = screenColor;

		// Ustawienie stanu Loading na pocz�tku fade out (przezroczysto�� -> czarny ekran)
		if (startAlpha < endAlpha)
		{
			GameState.STATE_LoadingLevel = true;
		}

		while (elapsedTime < fadeDuration)
		{
			elapsedTime += Time.unscaledDeltaTime; // U�ycie unscaledDeltaTime zamiast deltaTime
			float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);

			screenColor.a = newAlpha;
			blackScreenImage.color = screenColor;

			yield return null;
		}

		// Upewnienie si�, �e ko�cowa warto�� alpha jest dok�adnie ustawiona
		screenColor.a = endAlpha;
		blackScreenImage.color = screenColor;

		// Ustawienie stanu Loading na ko�cu fade in (czarny ekran -> przezroczysto��)
		if (startAlpha > endAlpha)
		{
			GameState.STATE_LoadingLevel = false;
		}
	}
}
