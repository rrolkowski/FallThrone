using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    public static ObjectGrabber Instance;

    [Header("Interaction Settings")]
    [SerializeField] float _horizontalRange = 2.0f;  // Radius range on the horizontal directions (X-Z)
    [SerializeField] float _verticalRange = 10.0f;    // Height range from the player  (Y)
    [SerializeField] string _interactableTag = "Interactable";
    [SerializeField] string _enemyInteractableTag = "Enemy";
    [SerializeField] LayerMask _groundLayer;

    [Header("Grab/Throw Settings")]
    [SerializeField] float _throwSpeedXZ = 10.0f; // Throw Speed
    [SerializeField] float _maxThrowRange = 10f; // Throw Range

    [Header("Objects")]
    [SerializeField] Transform _grabPoint;
    [SerializeField] Transform _originalParent;

    [Header("Scripts")]
    [SerializeField] RangeCircleController _rangeCircleController;

    [Header("")]
    [SerializeField] public GameObject currentlyGrabbedObject = null;


    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (_rangeCircleController != null)
        {
            _rangeCircleController.DeactivateRangeCircle();
        }
    }

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
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000.0f);

        //Draw a Raycast
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1000.0f, Color.red, 2.0f);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Raycast hit the object: " + hit.collider.name);

            if (hit.collider.CompareTag(_interactableTag) || hit.collider.CompareTag(_enemyInteractableTag))
            {
                Vector3 closestPoint = hit.collider.ClosestPoint(transform.position);

                // Calculate the horizontal distance from the nearest point on the collider to the player
                Vector3 playerPosition = transform.position;
                float horizontalDistance = Vector2.Distance(
                    new Vector2(playerPosition.x, playerPosition.z),
                    new Vector2(closestPoint.x, closestPoint.z)
                );

                if (horizontalDistance <= _horizontalRange)
                {
                    // We check that the object is in the right height range
                    float verticalDistance = Mathf.Abs(closestPoint.y - playerPosition.y);

                    if (verticalDistance <= _verticalRange)
                    {
                        Debug.Log("The object is in the range: " + hit.collider.name);

                        currentlyGrabbedObject = hit.collider.gameObject;

                        // If grabbing an enemy, stop its movement temporarily
                        if (currentlyGrabbedObject.TryGetComponent(out EnemyMovement enemy))
                        {
                            Vector3 enemyPosition = enemy.transform.position;
                            Vector3Int tilePosition = PathFinderManager.Instance.gridManager.tilemap.WorldToCell(enemyPosition);

                            enemy.isMovable = false;
                            Debug.Log("Enemy movement halted during grabbing");
                        }

                        currentlyGrabbedObject.transform.position = _grabPoint.position;


                        if (currentlyGrabbedObject.TryGetComponent(out Rigidbody rb))
                        {
                            rb.isKinematic = true;
                        }

                        if (currentlyGrabbedObject.TryGetComponent(out BoxCollider boxCollider))
                        {
                            boxCollider.isTrigger = true;
                        }

                        currentlyGrabbedObject.transform.SetParent(_grabPoint);
                        if (_rangeCircleController != null)
                        {
                            _rangeCircleController.ActivateRangeCircle();
                        }

                        if (currentlyGrabbedObject.TryGetComponent(out ThrowableObject throwable))
                        {
                            throwable.SetObjectAlpha(0.5f);
                        }
                        break;
                    }
                    else
                    {
                        Debug.Log("The object is outside the vertical range");
                    }
                }
                else
                {
                    Debug.Log("The object is outside the horizontal range");
                }
            }
            else
            {
                Debug.Log("Raycast hit object, but no 'Interactable' tag");
            }
        }
        if (currentlyGrabbedObject == null)
        {
            Debug.Log("Brak obiektów w zasiêgu chwycenia");
        }
    }

    void ReleaseObject()
    {
        if (currentlyGrabbedObject != null)
        {

            if (currentlyGrabbedObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
            }

            currentlyGrabbedObject.transform.SetParent(_originalParent);

            if (currentlyGrabbedObject.TryGetComponent(out BoxCollider boxCollider))
            {
                boxCollider.isTrigger = false;
            }

            Vector3 newPosition = currentlyGrabbedObject.transform.position;
            newPosition.y = transform.position.y;
            currentlyGrabbedObject.transform.position = newPosition;

            if (currentlyGrabbedObject.TryGetComponent(out ThrowableObject throwable))
            {
                throwable.SetObjectAlpha(1.0f);
            }

            currentlyGrabbedObject = null;
            if (_rangeCircleController != null)
            {
                _rangeCircleController.DeactivateRangeCircle();
            }
        }
    }

    void ThrowObject()
    {
        if (currentlyGrabbedObject != null)
        {
            if (currentlyGrabbedObject.TryGetComponent(out Rigidbody rb))
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;

                float maxThrowDistance = 250.0f;

                if (Physics.Raycast(ray, out hit, maxThrowDistance, _groundLayer))
                {
                    Vector3 targetPoint = hit.point;

                    // Calculate the direction and distance to the target point
                    Vector3 direction = targetPoint - _grabPoint.position;
                    float distance = direction.magnitude;

                    if (distance > _maxThrowRange)
                    {
                        Debug.Log("Target point out of range - throw canceled");
                        return;
                    }

                    GameObject thrownObject = currentlyGrabbedObject;
                    thrownObject.transform.SetParent(_originalParent);

                    if (thrownObject.TryGetComponent(out BoxCollider boxCollider))
                    {
                        boxCollider.isTrigger = false;
                    }

                    rb.isKinematic = false;

                    // calculate the time to reach the point.
                    float time = distance / _throwSpeedXZ;

                    // Set the raycast speed in the XZ direction
                    Vector3 throwVelocity = new Vector3(direction.x / time, 0, direction.z / time);

                    // kinematic equation for motion with acceleration, with gravity
                    float gravity = Mathf.Abs(Physics.gravity.y);
                    throwVelocity.y = (direction.y + 0.5f * gravity * Mathf.Pow(time, 2)) / time;

                    Debug.Log("Throw velocity: " + throwVelocity);

                    rb.linearVelocity = throwVelocity;

                    if (thrownObject.TryGetComponent(out ThrowableObject throwable))
                    {
                        throwable.SetObjectAlpha(0.5f);
                    }

                    currentlyGrabbedObject = null;
                    if (_rangeCircleController != null)
                    {
                        _rangeCircleController.DeactivateRangeCircle();
                    }
                }
                else
                {
                    Debug.Log("Raycast nie trafi³ w ¿adn¹ powierzchniê ziemi w zasiêgu");
                }
            }
        }
    }
   
    //Draw yellow cylinder
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        float radius = 2.0f;
        float height = 10.0f;

        Vector3 basePosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 topPosition = basePosition + Vector3.up * height;

        Gizmos.DrawWireSphere(basePosition, radius);
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        foreach (var direction in directions)
        {
            Vector3 offset = direction * radius;
            Gizmos.DrawLine(basePosition + offset, topPosition + offset);
        }

        // Draw Red disk
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, _maxThrowRange);

    }

}

