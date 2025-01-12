using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	[System.Serializable]
	public class UIElement
	{
		public string name;
		public GameObject canvas;
		public int priority; // Im wy¿sza wartoœæ, tym wy¿szy priorytet
	}

	public List<UIElement> uiElements;

	private Dictionary<string, UIElement> uiDictionary;
	private UIElement activeUI;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // Opcjonalne
		}
		else
		{
			Destroy(gameObject);
		}

		InitializeUIDictionary();
	}

	private void InitializeUIDictionary()
	{
		uiDictionary = new Dictionary<string, UIElement>();
		foreach (var element in uiElements)
		{
			if (!uiDictionary.ContainsKey(element.name))
			{
				uiDictionary.Add(element.name, element);
				element.canvas.SetActive(false); // Wy³¹cz wszystkie kanwasy na start
			}
		}
	}

	public void ShowUI(string name)
	{
		if (!uiDictionary.TryGetValue(name, out var uiElement))
		{
			Debug.LogWarning($"UI element with name {name} not found!");
			return;
		}

		// SprawdŸ priorytet
		if (activeUI != null && activeUI.priority >= uiElement.priority)
		{
			Debug.Log($"Cannot open {name}, because {activeUI.name} has higher or equal priority.");
			return;
		}

		// Zamknij aktualny UI, jeœli istnieje
		if (activeUI != null)
		{
			HideUI(activeUI.name);
		}

		// Otwórz nowe UI
		uiElement.canvas.SetActive(true);
		activeUI = uiElement;
	}

	public void HideUI(string name)
	{
		if (!uiDictionary.TryGetValue(name, out var uiElement))
		{
			Debug.LogWarning($"UI element with name {name} not found!");
			return;
		}

		if (activeUI == uiElement)
		{
			activeUI = null;
		}

		uiElement.canvas.SetActive(false);
	}

	public void HideAllUI()
	{
		foreach (var element in uiDictionary.Values)
		{
			element.canvas.SetActive(false);
		}
		activeUI = null;
	}

	public bool IsUIActive(string name)
	{
		return activeUI != null && activeUI.name == name;
	}
}
