using UnityEngine;

public class GameState : MonoBehaviour
{
	private static GameState Instance;

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

	public bool stateMainMenu;
	public bool stateGame;
	public bool statePaused;
	public bool stateShop;
	public bool stateLoadingLevel;
	public bool stateWon;
	public bool stateLost;

	public static bool STATE_MainMenu
	{
		get => Instance.stateMainMenu;
		set => Instance.stateMainMenu = value;
	}

	public static bool STATE_Game
	{
		get => Instance.stateGame;
		set => Instance.stateGame = value;
	}

	public static bool STATE_Paused
	{
		get => Instance.statePaused;
		set => Instance.statePaused = value;
	}

	public static bool STATE_Shop
	{
		get => Instance.stateShop;
		set => Instance.stateShop = value;
	}

	public static bool STATE_LoadingLevel
	{
		get => Instance.stateLoadingLevel;
		set => Instance.stateLoadingLevel = value;
	}

	public static bool STATE_Won
	{
		get => Instance.stateWon;
		set => Instance.stateWon = value;
	}

	public static bool STATE_Lost
	{
		get => Instance.stateLost;
		set => Instance.stateLost = value;
	}

	public static void ResetAllStates()
	{
		if (Instance == null) return;

		Instance.stateMainMenu = false;
		Instance.stateGame = false;
		Instance.statePaused = false;
		Instance.stateShop = false;
		Instance.stateLoadingLevel = false;
		Instance.stateWon = false;
		Instance.stateLost = false;
	}
}
