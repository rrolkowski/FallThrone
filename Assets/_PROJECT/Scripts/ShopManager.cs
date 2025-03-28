using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
	public static ShopManager Instance;

	public GameObject GUI_Shop;

	public GameObject buildingPrefab1;
	public GameObject buildingPrefab2;
	public GameObject buildingPrefab3;

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
			// Sprawdzenie, czy menu pauzy jest aktywne
			var gameController = GameController.Instance;
			if (GameState.STATE_Paused || GameState.STATE_Lost || GameState.STATE_Won || ObjectGrabber.Instance.isHoldingObject)
			{
				Debug.Log("Cannot open shop!");
				return;
			}

			ToggleShop();
		}
	}

	public void ToggleShop()
	{
		GameState.STATE_Shop = !GameState.STATE_Shop;
		Time.timeScale = GameState.STATE_Shop ? 0f : 1f;
		GUI_Shop.SetActive(GameState.STATE_Shop);
	}

	public void BuyTower1()
	{
		HandlePurchase(100, buildingPrefab1, "normal");
	}

	public void BuyTower2()
	{
		HandlePurchase(150, buildingPrefab2, "ice");
	}

	public void BuyTower3()
	{
		HandlePurchase(200, buildingPrefab3, "aoe");
	}

	private void HandlePurchase(int cost, GameObject prefab, string towerType)
	{
		var gameController = GameController.Instance;
		if (gameController == null || gameController.points < cost || gameController.currentTowers >= gameController.maxTowers)
			return;

		Debug.Log($"BuyTower {prefab.name}");

		gameController.RemovePoints(cost);
		gameController.AddTower();

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
}
