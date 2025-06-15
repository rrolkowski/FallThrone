using UnityEngine;
using UnityEngine.UI;

public class LevelStarsDisplay : MonoBehaviour
{
	[Header("Stars Holders for Levels 1-9 (assign 9 Transforms in order)")]
	public Transform[] starsHolders = new Transform[9]; // index 0 → Level 1, index 1 → Level 2, etc.

	[Header("Sprites")]
	public Sprite activeStarSprite;
	public Sprite inactiveStarSprite;

	private void Start()
	{
		UpdateAllStars();
	}

	private void OnEnable()
	{
		UpdateAllStars();
	}

	private void Update()
	{
		// Debug — klawisz = wypisuje ile gwiazdek dla wszystkich leveli
		if (Input.GetKeyDown(KeyCode.Equals))
		{
			Debug.Log("[DEBUG] PlayerPrefs stars:");
			for (int level = 1; level <= 9; level++)
			{
				int stars = PlayerPrefs.GetInt($"Level_{level}_Stars", 0);
				Debug.Log($"Level {level} stars = {stars}");
			}
		}
	}

	public void UpdateAllStars()
	{
		for (int i = 0; i < starsHolders.Length; i++)
		{
			int levelNumber = i + 1; // bo index 0 → Level 1
			int stars = PlayerPrefs.GetInt($"Level_{levelNumber}_Stars", 0);

			Transform holder = starsHolders[i];
			if (holder == null)
			{
				Debug.LogWarning($"[LevelStarsDisplay] Missing starsHolder for Level {levelNumber}!");
				continue;
			}

			// 🚀 NOWE: Wyłącz holder gdy 0 gwiazdek, włącz gdy >=1
			holder.gameObject.SetActive(stars > 0);

			// Jeśli holder wyłączony — nie trzeba ustawiać gwiazdek
			if (stars == 0) continue;

			for (int j = 0; j < holder.childCount; j++)
			{
				Image starImage = holder.GetChild(j).GetComponent<Image>();

				if (starImage != null)
				{
					starImage.sprite = (j < stars) ? activeStarSprite : inactiveStarSprite;
				}
				else
				{
					Debug.LogWarning($"Child {j} of {holder.name} has no Image component!");
				}
			}
		}
	}


	// public — można wywołać np. z UI "refresh button"
	public void ForceUpdateStars()
	{
		UpdateAllStars();
	}
}
