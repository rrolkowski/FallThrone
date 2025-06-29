using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Tutorial : MonoBehaviour
{
	public enum LevelType
	{
		Level1,
		Level4,
		Level7,
		Level8
	}

	public LevelType selectedLevel;

	public TextMeshProUGUI tutorialText;
	public TextMeshProUGUI headerText;
	public TextMeshProUGUI subHeaderText;
	public GameObject tutorialCanvas;
	public float fadeDuration = 0.5f;

	private string[] steps;
	private int currentStep = 0;
	private bool isTransitioning = false;
	private Material mat;

	private void Start()
	{
		if (tutorialCanvas != null)
			tutorialCanvas.SetActive(false);

		mat = tutorialText.fontMaterial;

		InitializeSteps();
	}

	private void InitializeSteps()
	{
		switch (selectedLevel)
		{
			case LevelType.Level1:
				steps = new string[]
				{
					"Your goal is to <color=#FFE300>defeat all enemies before they reach the end of the path.</color> Each enemy that reaches the end will cost you 1 health point.",
					"To win - <color=#FFE300>grab enemies or towers and throw them away from the endpoint</color> using the <color=#FFE300>LMB</color>.",
					"You can move with <color=#FFE300>WASD</color>, sprint with <color=#FFE300>SHIFT</color>, and open the shop with <color=#FFE300>SPACE</color> to buy new towers. Each tower costs both currency and tower points.",
					"<color=#FFE300>Defeating enemies earns you currency</color>, which you can use to buy more towers. You can also sell towers by grabbing them with <color=#FFE300>LMB</color> and pressing <color=#FFE300>Q</color>.",
					"Keep in mind: sprinting, grabbing enemies or towers, and throwing them all <color=#FFE300>consume stamina</color>.",
					"You can check all keybindings in the Pause Menu (<color=#FFE300>ESC</color>).",
					"<color=#FFE300>Good luck!</color>"
				};
				break;

			case LevelType.Level4:
				steps = new string[]
				{
					"A new tower is available in the shop — the <color=#FFE300>Ice Tower</color>! Use it to <color=#FFE300>slow down your enemies</color> and gain an advantage.",
					"A new enemy type has appeared — the <color=#FFE300>Ghost</color>. Be careful! <color=#FFE300>You can only grab it ONCE</color>, so plan your moves wisely.",
					"Brand new feature — <color=#FFE300>Power-Ups</color>! Collect Power-Ups that randomly appear on the ground and activate them by pressing <color=#FFE300>E</color>.",
					"<color=#FFE300>Good luck!</color>"
				};
				break;

			case LevelType.Level7:
				steps = new string[]
				{
					"A new tower is now available in the shop — the <color=#FFE300>Fire Tower</color>! This tower <color=#FFE300>deals area damage</color>, making it perfect for handling groups of enemies.",
					"A new type of Power-Up has been introduced — <color=#FFE300>the Range Boost</color>! Pick it up, activate it by pressing <color=#FFE300>E</color>, and throw enemies even further than before.",
					"<color=#FFE300>Good luck!</color>"
				};
				break;

			case LevelType.Level8:
				steps = new string[]
				{
					"A new enemy type has appeared - <color=#FFE300>Turbo Bruno</color>. Watch out! To defeat this enemy, you must first <color=#FFE300>destroy his shield</color>.",
					"To break the shield, grab him and throw him into the <color=#FFE300>Blue Statue!</color>",
					"<color=#FFE300>Good luck!</color>"
				};
				break;

			default:
				steps = new string[] { "Default tutorial text." };
				break;
		}
	}

	private void Update()
	{
		if (GameController.Instance.isTutorialActive)
		{
			if (tutorialCanvas != null && !tutorialCanvas.activeSelf)
			{
				tutorialCanvas.SetActive(true);
				currentStep = 0;
				StartCoroutine(FadeTextAlpha(headerText, 1f));
				StartCoroutine(FadeTextAlpha(subHeaderText, 1f));
				StartCoroutine(FadeTextStep(steps[currentStep]));
			}

			if (!isTransitioning && (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
			{
				currentStep++;

				if (currentStep < steps.Length)
				{
					StartCoroutine(FadeTextStep(steps[currentStep]));
				}
				else
				{
					GameController.Instance.isTutorialActive = false;
					StartCoroutine(EndTutorial());
				}
			}
		}
	}

	private IEnumerator FadeTextStep(string newText)
	{
		isTransitioning = true;

		yield return StartCoroutine(FadeTextMaterialAlpha(mat, 0f));

		tutorialText.text = newText;

		yield return StartCoroutine(FadeTextMaterialAlpha(mat, 1f));

		isTransitioning = false;
	}

	private IEnumerator FadeTextMaterialAlpha(Material targetMat, float targetAlpha)
	{
		Color faceColor = targetMat.GetColor("_FaceColor");
		float startAlpha = faceColor.a;
		float time = 0f;

		while (time < fadeDuration)
		{
			time += Time.deltaTime;
			float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
			faceColor.a = alpha;
			targetMat.SetColor("_FaceColor", faceColor);
			yield return null;
		}

		faceColor.a = targetAlpha;
		targetMat.SetColor("_FaceColor", faceColor);
	}

	private IEnumerator FadeTextAlpha(TextMeshProUGUI text, float targetAlpha)
	{
		Color color = text.color;
		float startAlpha = color.a;
		float time = 0f;

		while (time < fadeDuration)
		{
			time += Time.deltaTime;
			float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
			color.a = alpha;
			text.color = color;
			yield return null;
		}

		color.a = targetAlpha;
		text.color = color;
	}

	private IEnumerator EndTutorial()
	{
		isTransitioning = true;

		// Rozpoczynamy fade wszystkich tekstów jednoczeœnie
		Coroutine fadeMain = StartCoroutine(FadeTextMaterialAlpha(mat, 0f));
		Coroutine fadeHeader = StartCoroutine(FadeTextAlpha(headerText, 0f));
		Coroutine fadeSubHeader = StartCoroutine(FadeTextAlpha(subHeaderText, 0f));

		// Czekamy na fade g³ównego tekstu, zak³adaj¹c ¿e wszystkie maj¹ ten sam czas
		yield return fadeMain;
		yield return fadeHeader;
		yield return fadeSubHeader;

		if (tutorialCanvas != null)
			tutorialCanvas.SetActive(false);

		isTransitioning = false;
	}
}
