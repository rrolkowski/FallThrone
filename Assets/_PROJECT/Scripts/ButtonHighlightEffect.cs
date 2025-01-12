using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ButtonHighlightEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private Transform targetObject; // Obiekt, którego skala ma siê zmieniaæ
	[SerializeField] private float scaleMultiplier = 0.95f;
	[SerializeField] private Color highlightColor = Color.yellow; // Kolor podœwietlenia
	[SerializeField] private Color originalColor = Color.white; // Kolor bazowy
	[SerializeField] private float scaleAnimationDuration = 0.2f; // Czas trwania animacji
	[SerializeField] private bool enableSmoothAnimation = false; // W³¹czanie p³ynnej animacji

	private Vector3 originalScale;
	private Coroutine scaleCoroutine;

	private void Awake()
	{
		// Jeœli nie przypisano targetObject, u¿yj bie¿¹cego obiektu
		if (targetObject == null)
		{
			targetObject = transform;
		}

		// Zapisanie oryginalnej skali obiektu
		originalScale = targetObject.localScale;
	}

	private void OnDisable()
	{
		// Zatrzymanie bie¿¹cej korutyny, jeœli dzia³a
		if (scaleCoroutine != null)
		{
			StopCoroutine(scaleCoroutine);
			scaleCoroutine = null;
		}

		// Powrót do oryginalnej skali
		targetObject.localScale = originalScale;

		// Przywrócenie oryginalnego koloru
		ChangeColors(targetObject, originalColor);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		AudioManager.PlaySound(SoundType.MENU_Highlight_Button);

		if (enableSmoothAnimation)
		{
			if (scaleCoroutine != null)
			{
				StopCoroutine(scaleCoroutine);
			}
			scaleCoroutine = StartCoroutine(SmoothScale(targetObject.localScale, originalScale * scaleMultiplier));
		}
		else
		{
			targetObject.localScale = originalScale * scaleMultiplier;
		}

		// Zmiana koloru dla wszystkich obiektów w kontenerze
		ChangeColors(targetObject, highlightColor);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (enableSmoothAnimation)
		{
			if (scaleCoroutine != null)
			{
				StopCoroutine(scaleCoroutine);
			}
			scaleCoroutine = StartCoroutine(SmoothScale(targetObject.localScale, originalScale));
		}
		else
		{
			targetObject.localScale = originalScale;
		}

		// Przywrócenie oryginalnego koloru
		ChangeColors(targetObject, originalColor);
	}

	private void ChangeColors(Transform container, Color color)
	{
		// Iteracja po wszystkich dzieciach kontenera
		foreach (Transform child in container)
		{
			// SprawdŸ, czy obiekt ma komponent Image
			Image image = child.GetComponent<Image>();
			if (image != null)
			{
				image.color = color;
			}

			// SprawdŸ, czy obiekt ma komponent TextMeshProUGUI
			TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
			if (text != null)
			{
				text.color = color;
			}

			// Rekurencyjnie obs³u¿ potencjalne pod-kontenery
			if (child.childCount > 0)
			{
				ChangeColors(child, color);
			}
		}
	}

	private IEnumerator SmoothScale(Vector3 from, Vector3 to)
	{
		float elapsedTime = 0f;

		while (elapsedTime < scaleAnimationDuration)
		{
			targetObject.localScale = Vector3.Lerp(from, to, elapsedTime / scaleAnimationDuration);
			elapsedTime += Time.unscaledDeltaTime; // U¿ycie Time.unscaledDeltaTime zamiast Time.deltaTime
			yield return null;
		}

		targetObject.localScale = to;
	}

}
