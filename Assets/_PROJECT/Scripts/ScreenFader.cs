using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
	public static ScreenFader Instance;

	public Image blackScreenImage; // Obraz czarnego ekranu (ustawiæ w Inspectorze)
	public float fadeDuration = 1.0f; // Czas trwania przejœcia (w sekundach)

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

	// Publiczna funkcja do wywo³ania fade in (czarny ekran -> przezroczystoœæ)
	public void FadeIn()
	{
		StartCoroutine(Fade(1, 0));
	}

	// Publiczna funkcja do wywo³ania fade out (przezroczystoœæ -> czarny ekran)
	public void FadeOut()
	{
		StartCoroutine(Fade(0, 1));
	}

	// Funkcja odpowiedzialna za animacjê alpha
	private IEnumerator Fade(float startAlpha, float endAlpha)
	{
		float elapsedTime = 0f;
		Color screenColor = blackScreenImage.color;

		// Ustawienie pocz¹tkowej wartoœci alpha
		screenColor.a = startAlpha;
		blackScreenImage.color = screenColor;

		// Ustawienie stanu Loading na pocz¹tku fade out (przezroczystoœæ -> czarny ekran)
		if (startAlpha < endAlpha)
		{
			GameState.STATE_LoadingLevel = true;
		}

		while (elapsedTime < fadeDuration)
		{
			elapsedTime += Time.unscaledDeltaTime; // U¿ycie unscaledDeltaTime zamiast deltaTime
			float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);

			screenColor.a = newAlpha;
			blackScreenImage.color = screenColor;

			yield return null;
		}

		// Upewnienie siê, ¿e koñcowa wartoœæ alpha jest dok³adnie ustawiona
		screenColor.a = endAlpha;
		blackScreenImage.color = screenColor;

		// Ustawienie stanu Loading na koñcu fade in (czarny ekran -> przezroczystoœæ)
		if (startAlpha > endAlpha)
		{
			GameState.STATE_LoadingLevel = false;
		}
	}
}
