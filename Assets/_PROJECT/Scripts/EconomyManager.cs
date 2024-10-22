using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
	public static EconomyManager Instance;

	public int points = 0; // Liczba punktów
	public TextMeshProUGUI pointsText;

	private int lastPointsValue;

	private void Awake()
	{
		Instance = this;
		UpdatePointsText();
		lastPointsValue = points;
	}

	private void Update()
	{
		// SprawdŸ, czy wartoœæ punktów siê zmieni³a
		if (points != lastPointsValue)
		{
			UpdatePointsText();
			lastPointsValue = points;
		}
	}

	// Funkcja do aktualizacji tekstu w TextMeshProUGUI
	private void UpdatePointsText()
	{
		if (pointsText != null)
		{
			pointsText.text = "[ " + points.ToString() + " POINTS ]";
		}
	}
}
