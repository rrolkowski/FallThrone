using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
	// Prywatne zmienne sceny
	[SerializeField] private string menuScene; // Nazwa sceny menu
	[SerializeField] private string[] levelScenes; // Tablica nazw poziomów

	private static SceneManagerScript instance;

	private void Awake()
	{
		// Singleton, aby umo¿liwiæ dostêp z innych skryptów
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

	/// <summary>
	/// Instancja Singletona, pozwala na dostêp do metod tego skryptu.
	/// </summary>
	public static SceneManagerScript Instance => instance;

	/// <summary>
	/// £aduje scenê menu.
	/// </summary>
	public void LoadMenuScene()
	{
		if (!string.IsNullOrEmpty(menuScene))
		{
			SceneManager.LoadScene(menuScene);
			UpdateGameState(menuScene);
		}
		else
		{
			Debug.LogError("MenuScene nie jest przypisana w edytorze.");
		}
	}

	/// <summary>
	/// £aduje poziom na podstawie indeksu w tablicy levelScenes.
	/// </summary>
	/// <param name="levelIndex">Indeks poziomu (np. 0 dla Level1, 1 dla Level2).</param>
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
			else
			{
				Debug.LogError($"Level {levelIndex} nie jest przypisany w edytorze.");
			}
		}
		else
		{
			Debug.LogError("Level index poza zakresem.");
		}
	}

	/// <summary>
	/// Prze³adowuje bie¿¹c¹ scenê.
	/// </summary>
	public void ReloadCurrentScene()
	{
		Scene currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(currentScene.name);
		UpdateGameState(currentScene.name);
	}

	/// <summary>
	/// Aktualizuje stany gry w zale¿noœci od nazwy sceny.
	/// </summary>
	/// <param name="sceneName">Nazwa sceny.</param>
	private void UpdateGameState(string sceneName)
	{
		GameState.ResetAllStates();

		// Ustaw odpowiedni stan na true
		if (sceneName == menuScene)
		{
			GameState.STATE_MainMenu = true;
		}
		else if (System.Array.Exists(levelScenes, scene => scene == sceneName))
		{
			GameState.STATE_Game = true;
		}
		else
		{
			Debug.LogWarning("Nie rozpoznano sceny: " + sceneName);
		}
	}
}
