using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ButtonHighlightEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public enum TowerType
	{
		NormalTower,
		IceTower,
		AOETower
	}

	[Header("Target Settings")]
	[SerializeField] private Transform targetObject;
	[SerializeField] private float scaleMultiplier = 0.95f;
	[SerializeField] private Color highlightColor = Color.yellow;
	[SerializeField] private Color originalColor = Color.white;
	[SerializeField] private float scaleAnimationDuration = 0.2f;
	[SerializeField] private bool enableSmoothAnimation = false;

	[Header("Tower Button Settings")]
	[SerializeField] private bool isTowerButton = false; // <-- Nowy bool
	[SerializeField] private TowerType towerType;
	[SerializeField] private TextMeshProUGUI descriptionText;

	private Vector3 originalScale;
	private Coroutine scaleCoroutine;

	private void Awake()
	{
		if (targetObject == null)
		{
			targetObject = transform;
		}

		originalScale = targetObject.localScale;

		// Jeœli to przycisk wie¿y, upewnij siê, ¿e opis zosta³ przypisany
		if (isTowerButton && descriptionText == null)
		{
			Debug.LogWarning($"Button '{gameObject.name}' is marked as TowerButton but has no DescriptionText assigned!", this);
		}
	}

	private void OnDisable()
	{
		if (scaleCoroutine != null)
		{
			StopCoroutine(scaleCoroutine);
			scaleCoroutine = null;
		}

		targetObject.localScale = originalScale;
		ChangeColors(targetObject, originalColor);

		// Opcjonalne czyszczenie opisu
		// if (isTowerButton && descriptionText != null) descriptionText.text = "";
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

		ChangeColors(targetObject, highlightColor);

		// Aktualizacja opisu tylko dla przycisków wie¿
		if (isTowerButton)
		{
			UpdateDescriptionText();
		}
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

		ChangeColors(targetObject, originalColor);

		// Opcjonalne czyszczenie opisu
		// if (isTowerButton && descriptionText != null) descriptionText.text = "";
	}

	private void ChangeColors(Transform container, Color color)
	{
		foreach (Transform child in container)
		{
			Image image = child.GetComponent<Image>();
			if (image != null)
			{
				image.color = color;
			}

			TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
			if (text != null)
			{
				text.color = color;
			}

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
			elapsedTime += Time.unscaledDeltaTime;
			yield return null;
		}

		targetObject.localScale = to;
	}

	private void UpdateDescriptionText()
	{
		if (descriptionText == null)
			return;

		switch (towerType)
		{
			case TowerType.NormalTower:
				descriptionText.text = "A basic tower that shoots fireballs at one enemy at a time. Always attacks the enemy with the lowest health.";
				break;
			case TowerType.IceTower:
				descriptionText.text = "A tower that shoots ice projectiles at one enemy. Always targets the nearest enemy.";
				break;
			case TowerType.AOETower:
				descriptionText.text = "A tower that attacks all nearby enemies with an area of effect attack.";
				break;
		}
	}
}
