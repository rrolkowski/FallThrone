using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
	[SerializeField] private string menuScene;
	[SerializeField] private string[] levelScenes;

	private static SceneManagerScript instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		Scene currentScene = SceneManager.GetActiveScene();
		UpdateGameState(currentScene.name);
	}

	public static SceneManagerScript Instance => instance;

	public string[] LevelScenes => levelScenes;

	public void LoadMenuScene()
	{
		if (!string.IsNullOrEmpty(menuScene))
		{
			SceneManager.LoadScene(menuScene);
			UpdateGameState(menuScene);
		}
	}

	public void LoadLevel(int levelIndex)
	{
		if (levelIndex >= 0 && levelIndex < levelScenes.Length)
		{
			string sceneName = levelScenes[levelIndex];
			if (!string.IsNullOrEmpty(sceneName))
			{
				SceneManager.LoadScene(sceneName);
				UpdateGameState(sceneName);
			}
		}
	}

	public void ReloadCurrentScene()
	{
		Scene currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(currentScene.name);
		UpdateGameState(currentScene.name);
	}

	private void UpdateGameState(string sceneName)
	{
		GameState.ResetAllStates();

		if (sceneName == menuScene)
		{
			GameState.STATE_MainMenu = true;
		}
		else if (System.Array.Exists(levelScenes, scene => scene == sceneName))
		{
			GameState.STATE_Game = true;
		}
	}

	public void LoadNextLevel()
	{
		string currentSceneName = SceneManager.GetActiveScene().name;
		int currentIndex = System.Array.IndexOf(levelScenes, currentSceneName);

		if (currentIndex != -1 && currentIndex + 1 < levelScenes.Length)
		{
			string nextSceneName = levelScenes[currentIndex + 1];
			if (!string.IsNullOrEmpty(nextSceneName))
			{
				SceneManager.LoadScene(nextSceneName);
				UpdateGameState(nextSceneName);
			}
		}
	}

	public void UnlockNextLevel()
	{
		string currentSceneName = SceneManager.GetActiveScene().name;
		int currentIndex = System.Array.IndexOf(levelScenes, currentSceneName);
		int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);

		if (currentIndex + 1 > unlockedLevel && currentIndex + 1 < levelScenes.Length)
		{
			PlayerPrefs.SetInt("UnlockedLevel", currentIndex + 1);
			PlayerPrefs.Save();
		}
	}
}
