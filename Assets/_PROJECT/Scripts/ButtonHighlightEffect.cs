using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

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
	[SerializeField] private float animationSpeed = 10f;

	[Header("Tower Button Settings")]
	[SerializeField] private bool isTowerButton = false;
	[SerializeField] private TowerType towerType;
	[SerializeField] private TextMeshProUGUI descriptionText;

	[Header("Solo Text Mode")]
	[SerializeField] private bool soloText = false;

	private Vector3 originalScale;
	private Vector3 targetScale;

	private TextMeshProUGUI targetText;
	private float originalFontSize;
	private float targetFontSize;
	private Color targetTextColor;
	private Color currentTextColor;

	private bool isHovered = false;

	private void Awake()
	{
		if (targetObject == null)
			targetObject = transform;

		originalScale = targetObject.localScale;
		targetScale = originalScale;

		if (soloText)
		{
			targetText = targetObject.GetComponent<TextMeshProUGUI>();
			if (targetText != null)
			{
				originalFontSize = targetText.fontSize;
				targetFontSize = originalFontSize;
				currentTextColor = targetText.color;
				targetTextColor = currentTextColor;
			}
		}

		if (isTowerButton && descriptionText == null)
		{
			Debug.LogWarning($"Button '{gameObject.name}' is marked as TowerButton but has no DescriptionText assigned!", this);
		}
	}

	private void OnDisable()
	{
		targetObject.localScale = originalScale;

		if (soloText && targetText != null)
		{
			targetText.fontSize = originalFontSize;
			targetText.color = originalColor;
			targetFontSize = originalFontSize;
			targetTextColor = originalColor;
		}
		else
		{
			ChangeColors(targetObject, originalColor);
			targetScale = originalScale;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (isTowerButton && !CanAffordTower())
			return;

		AudioManager.PlaySound(SoundType.MENU_Highlight_Button);

		isHovered = true;

		if (soloText && targetText != null)
		{
			targetFontSize = originalFontSize * 0.9f;
			targetTextColor = highlightColor;
		}
		else
		{
			targetScale = originalScale * scaleMultiplier;
			ChangeColors(targetObject, highlightColor);
		}

		if (isTowerButton)
		{
			UpdateDescriptionText();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovered = false;

		if (soloText && targetText != null)
		{
			targetFontSize = originalFontSize;
			targetTextColor = originalColor;
		}
		else
		{
			targetScale = originalScale;
			ChangeColors(targetObject, originalColor);
		}
	}

	private void Update()
	{
		if (soloText && targetText != null)
		{
			targetText.fontSize = Mathf.Lerp(targetText.fontSize, targetFontSize, Time.unscaledDeltaTime * animationSpeed);
			targetText.color = Color.Lerp(targetText.color, targetTextColor, Time.unscaledDeltaTime * animationSpeed);
		}
		else
		{
			targetObject.localScale = Vector3.Lerp(targetObject.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
		}
	}

	private void ChangeColors(Transform container, Color color)
	{
		foreach (Transform child in container)
		{
			if (child.TryGetComponent(out Image image))
			{
				image.color = color;
			}

			if (child.TryGetComponent(out TextMeshProUGUI text) && (!soloText || text != targetText))
			{
				text.color = color;
			}

			if (child.childCount > 0)
			{
				ChangeColors(child, color);
			}
		}
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

	private bool CanAffordTower()
	{
		if (GameController.Instance == null)
			return false;

		int points = GameController.Instance.points;
		int current = GameController.Instance.currentTower_Points;
		int max = GameController.Instance.maxTower_Points;

		return towerType switch
		{
			TowerType.NormalTower => points >= 100 && current + 2 <= max,
			TowerType.IceTower => points >= 150 && current + 1 <= max,
			TowerType.AOETower => points >= 200 && current + 3 <= max,
			_ => false
		};
	}
}
