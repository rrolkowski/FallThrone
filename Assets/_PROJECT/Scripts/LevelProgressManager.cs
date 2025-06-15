using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
	public static LevelProgressManager Instance;

	private void Awake()
	{
		// Singleton pattern
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // opcjonalnie — zostaje miêdzy scenami
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void SaveStarsForLevel(int levelNumber, int stars)
	{
		int clampedStars = Mathf.Clamp(stars, 0, 3);

		int previousStars = GetStarsForLevel(levelNumber);
		if (clampedStars > previousStars)
		{
			PlayerPrefs.SetInt($"Level_{levelNumber}_Stars", clampedStars);
			PlayerPrefs.Save();
			Debug.Log($"[LevelProgressManager] Saved {clampedStars} stars for Level {levelNumber}");
		}
		else
		{
			Debug.Log($"[LevelProgressManager] Previous stars ({previousStars}) >= new stars ({clampedStars}) — not saving.");
		}
	}

	public int GetStarsForLevel(int levelNumber)
	{
		int stars = PlayerPrefs.GetInt($"Level_{levelNumber}_Stars", 0);
		Debug.Log($"[LevelProgressManager] Loaded stars for Level {levelNumber}: {stars}");
		return stars;
	}

	public void ClearAllProgress()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		Debug.Log("[LevelProgressManager] All progress cleared!");
	}
}
