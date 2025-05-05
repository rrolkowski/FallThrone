using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
	public static ShopManager Instance;

	public GameObject GUI_Shop;

	public GameObject buildingPrefab1;
	public GameObject buildingPrefab2;
	public GameObject buildingPrefab3;

	[Header("UI Elements for Tower 1")]
	public Image tower1_Image1;
	public Image tower1_Image2;
	public TextMeshProUGUI tower1cost_Text;
	public TextMeshProUGUI tower1towerlimit_Text;

	[Header("UI Elements for Tower 2")]
	public Image tower2_Image1;
	public Image tower2_Image2;
	public TextMeshProUGUI tower2cost_Text;
	public TextMeshProUGUI tower2towerlimit_Text;

	[Header("UI Elements for Tower 3")]
	public Image tower3_Image1;
	public Image tower3_Image2;
	public TextMeshProUGUI tower3cost_Text;
	public TextMeshProUGUI tower3towerlimit_Text;

	[Header("Settings")]
	private Color affordableColor = Color.white;
	private Color notAffordableColor = Color.red;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		GUI_Shop.SetActive(false);
		GameState.STATE_Shop = false;
	}

	private void Update()
	{
		if (GameState.STATE_LoadingLevel) return;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			var gameController = GameController.Instance;
			if (GameState.STATE_Paused || GameState.STATE_Lost || GameState.STATE_Won || ObjectGrabber.Instance.isHoldingObject)
			{
				Debug.Log("Cannot open shop!");
				return;
			}

			ToggleShop();
		}

		UpdateShopUI();
	}

	public void ToggleShop()
	{
		GameState.STATE_Shop = !GameState.STATE_Shop;
		Time.timeScale = GameState.STATE_Shop ? 0f : 1f;
		GUI_Shop.SetActive(GameState.STATE_Shop);
	}

	public void BuyTower1()
	{
		HandlePurchase(100, buildingPrefab1, "normal", 2);
	}

	public void BuyTower2()
	{
		HandlePurchase(150, buildingPrefab2, "ice", 1);
	}

	public void BuyTower3()
	{
		HandlePurchase(200, buildingPrefab3, "aoe", 3);
	}

	private void HandlePurchase(int cost, GameObject prefab, string towerType, int tower_cost)
	{
		var gameController = GameController.Instance;
		if (gameController == null || gameController.points < cost || gameController.currentTower_Points + tower_cost > gameController.maxTower_Points)
			return;

		Debug.Log($"BuyTower {prefab.name}");

		gameController.RemovePoints(cost);
		gameController.AddTower(tower_cost);

		if (towerType == "normal")
		{
			AudioManager.PlaySound(SoundType.MENU_Turret_Building);
		}
		else if (towerType == "ice")
		{
			AudioManager.PlaySound(SoundType.MENU_Turret_Building2);
		}
		else if (towerType == "aoe")
		{
			AudioManager.PlaySound(SoundType.MENU_Turret_Building3);
		}

		ToggleShop();

		var objectGrabber = ObjectGrabber.Instance;
		if (objectGrabber != null)
		{
			objectGrabber.currentlyGrabbedObject = Instantiate(prefab, objectGrabber._grabPoint.position, Quaternion.identity, objectGrabber._grabPoint);
			var boxCollider = objectGrabber.currentlyGrabbedObject.GetComponent<BoxCollider>();
			if (boxCollider != null)
			{
				boxCollider.isTrigger = true;
			}
			objectGrabber._rangeCircleController.ActivateRangeCircle();
		}
	}

	private void UpdateShopUI()
	{
		var gameController = GameController.Instance;
		if (gameController == null) return;

		UpdateTowerUI(100, 2, tower1_Image1, tower1_Image2, tower1cost_Text, tower1towerlimit_Text);
		UpdateTowerUI(150, 1, tower2_Image1, tower2_Image2, tower2cost_Text, tower2towerlimit_Text);
		UpdateTowerUI(200, 3, tower3_Image1, tower3_Image2, tower3cost_Text, tower3towerlimit_Text);
	}

	private void UpdateTowerUI(int towerCostPoints, int towerCostTowers, Image img1, Image img2, TextMeshProUGUI costText, TextMeshProUGUI limitText)
	{
		var gameController = GameController.Instance;

		bool canAfford = gameController.points >= towerCostPoints;
		bool hasSpace = (gameController.currentTower_Points + towerCostTowers) <= gameController.maxTower_Points;

		// Jeœli nie staæ lub brak miejsca -> wyszarz
		bool shouldGrayOut = !canAfford || !hasSpace;

		SetUITransparency(img1, !shouldGrayOut);
		SetUITransparency(img2, !shouldGrayOut);

		// Ustaw kolor kosztu $ (points)
		SetUITextColor(costText, canAfford);

		// Ustaw kolor limitu wie¿ (tower points)
		SetUITextColor(limitText, hasSpace);
	}

	private void SetUITransparency(Image img, bool isActive)
	{
		if (img == null) return;

		Color color = img.color;
		color.a = isActive ? 1f : 10f / 255f;
		img.color = color;
	}

	private void SetUITextColor(TextMeshProUGUI text, bool conditionMet)
	{
		if (text == null) return;

		text.color = conditionMet ? affordableColor : notAffordableColor;
	}
}
