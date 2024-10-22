using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] float interactionRange = 2.0f;
    [SerializeField] string interactableTag = "Interactable";

    [Header("Grab/Throw Settings")]
    [SerializeField] float throwForce = 500f; // Throw power


    [Header("Objects")]
    [SerializeField] Transform grabPoint;
    [SerializeField] Transform originalParent;

    [SerializeField] public GameObject currentlyGrabbedObject = null;

    // Input System Method for the "Grab" action
    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
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
    }

    // Input System Method for the "Throww" action
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed && currentlyGrabbedObject != null)
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
                currentlyGrabbedObject = hitCollider.gameObject;
                currentlyGrabbedObject.transform.position = grabPoint.position;

                if (currentlyGrabbedObject.GetComponent<Rigidbody>())
                {
                    currentlyGrabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                }

                currentlyGrabbedObject.transform.SetParent(grabPoint);

                break;
            }
        }
    }

    void ReleaseObject()
    {
        if (currentlyGrabbedObject != null)
        {
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

    // Draw interaction range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

