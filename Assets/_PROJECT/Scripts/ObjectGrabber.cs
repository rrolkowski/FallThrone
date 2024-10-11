using UnityEngine;

public class ObjectGrabber : MonoBehaviour
{
	public float interactionRange = 2.0f;
	public string interactableTag = "Interactable";

	public Transform grabPoint;

	public float throwForce = 500f; // si³a rzutu

	public Transform originalParent;
	public GameObject currentlyGrabbedObject = null;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			if (currentlyGrabbedObject == null)
			{
				TryGrabObject();
			}
			else
			{
				ReleaseObject();
			}
		}

		if (Input.GetKeyDown(KeyCode.C) && currentlyGrabbedObject != null)
		{
			ThrowObject();
		}
	}

	void TryGrabObject()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
		foreach (Collider hitCollider in hitColliders)
		{
			if (hitCollider.CompareTag(interactableTag))
			{
				//Debug.Log("Podniesiono obiekt: " + hitCollider.gameObject.name);

				currentlyGrabbedObject = hitCollider.gameObject;
				currentlyGrabbedObject.transform.position = grabPoint.position;

				if (currentlyGrabbedObject.GetComponent<Rigidbody>())
				{
					currentlyGrabbedObject.GetComponent<Rigidbody>().isKinematic = true;
				}

				currentlyGrabbedObject.transform.SetParent(grabPoint);

				break;
			}
			else
			{
				//Debug.Log("Nie znaleziono w pobli¿u obiektu mo¿liwego do podniesienia.");
			}
		}
	}

	void ReleaseObject()
	{
		if (currentlyGrabbedObject != null)
		{
			//Debug.Log("Upuszczono obiekt: " + currentlyGrabbedObject.name);

			if (currentlyGrabbedObject.GetComponent<Rigidbody>())
			{
				currentlyGrabbedObject.GetComponent<Rigidbody>().isKinematic = false;
			}

			currentlyGrabbedObject.transform.SetParent(originalParent);

			Vector3 newPosition = currentlyGrabbedObject.transform.position;
			newPosition.y = transform.position.y;
			currentlyGrabbedObject.transform.position = newPosition;

			currentlyGrabbedObject = null;
		}
	}

	void ThrowObject()
	{
		if (currentlyGrabbedObject != null)
		{
			//Debug.Log("Rzucono obiekt: " + currentlyGrabbedObject.name);

			Rigidbody rb = currentlyGrabbedObject.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.isKinematic = false;

				currentlyGrabbedObject.transform.SetParent(originalParent);

				rb.AddForce(transform.forward * throwForce);

				currentlyGrabbedObject = null;
			}
		}
	}
}
