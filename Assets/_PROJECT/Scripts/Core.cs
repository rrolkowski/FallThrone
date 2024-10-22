using UnityEngine;

public class Core : MonoBehaviour
{
	public bool isPlayerTriggered = false;
	public GameObject tooltipText;
	public GameObject GUI_Shop;
	public bool isShopActive = false;

	public GameObject buildingPrefab1;
	public GameObject buildingPrefab2;
	public GameObject buildingPrefab3;

	public Transform spawnpointObject;

	private void Update()
	{
		ActiveTooltip(isPlayerTriggered);

		if (isPlayerTriggered && Input.GetKeyDown(KeyCode.Space))
		{
			ToggleShop();
		}
	}

	void ActiveTooltip(bool value)
	{
		tooltipText.SetActive(value);
	}

	// W³¹cz / Wy³¹cz GUI sklepu
	void ToggleShop()
	{
		if (isShopActive)
		{
			Time.timeScale = 1f;
			GUI_Shop.SetActive(false);
			isShopActive = false;
		}
		else
		{
			Time.timeScale = 0f;
			GUI_Shop.SetActive(true);
			isShopActive = true;
		}
	}

	//

	public void BuyBuilding1()
	{
		int points = EconomyManager.Instance.points;

		if (points >= 50)
		{
			Debug.Log("Kupi³eœ budynek 1");
			ToggleShop();

			// Zrespienie wybranego budynku z prefaba oraz przypisanie do od razu do zmiennej currentlyGrabbedObject ze skryptu ObjectGrabber
			ObjectGrabber.Instance.currentlyGrabbedObject = Instantiate(buildingPrefab1, spawnpointObject.position, Quaternion.identity, spawnpointObject);
			// Aby unikn¹æ problemu z kolizjami wy³¹czamy BoxCollider w momencie kupienia budynku
			ObjectGrabber.Instance.currentlyGrabbedObject.GetComponent<BoxCollider>().isTrigger = true;

			// Odejmij ustalon¹ iloœæ punktów za kupienie budynku
			EconomyManager.Instance.points -= 50;
		}
		else
		{
			Debug.Log("Nie masz wystarzcaj¹co du¿o punktów, aby kupiæ ten budynek!");
		}
	}

	public void BuyBuilding2()
	{
		int points = EconomyManager.Instance.points;

		if (points >= 100)
		{
			Debug.Log("Kupi³eœ budynek 2");
			ToggleShop();

			// Zrespienie wybranego budynku z prefaba oraz przypisanie do od razu do zmiennej currentlyGrabbedObject ze skryptu ObjectGrabber
			ObjectGrabber.Instance.currentlyGrabbedObject = Instantiate(buildingPrefab2, spawnpointObject.position, Quaternion.identity, spawnpointObject);
			// Aby unikn¹æ problemu z kolizjami wy³¹czamy BoxCollider w momencie kupienia budynku
			ObjectGrabber.Instance.currentlyGrabbedObject.GetComponent<BoxCollider>().isTrigger = true;

			// Odejmij ustalon¹ iloœæ punktów za kupienie budynku
			EconomyManager.Instance.points -= 100;
		}
		else
		{
			Debug.Log("Nie masz wystarzcaj¹co du¿o punktów, aby kupiæ ten budynek!");
		}
	}

	public void BuyBuilding3()
	{
		int points = EconomyManager.Instance.points;

		if (points >= 150)
		{
			Debug.Log("Kupi³eœ budynek 3");
			ToggleShop();

			// Zrespienie wybranego budynku z prefaba oraz przypisanie do od razu do zmiennej currentlyGrabbedObject ze skryptu ObjectGrabber
			ObjectGrabber.Instance.currentlyGrabbedObject = Instantiate(buildingPrefab3, spawnpointObject.position, Quaternion.identity, spawnpointObject);
			// Aby unikn¹æ problemu z kolizjami wy³¹czamy BoxCollider w momencie kupienia budynku
			ObjectGrabber.Instance.currentlyGrabbedObject.GetComponent<BoxCollider>().isTrigger = true;

			// Odejmij ustalon¹ iloœæ punktów za kupienie budynku
			EconomyManager.Instance.points -= 150;
		}
		else
		{
			Debug.Log("Nie masz wystarzcaj¹co du¿o punktów, aby kupiæ ten budynek!");
		}
	}

	//

	private void OnTriggerStay(Collider other)
	{
		if (isPlayerTriggered) return;

		if (other.CompareTag("Player"))
		{
			isPlayerTriggered = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerTriggered = false;
		}
	}
}
