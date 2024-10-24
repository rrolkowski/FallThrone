using UnityEngine.InputSystem;
using UnityEngine;

public class RangeCircleController : MonoBehaviour
{
    [Header("Range Settings")]
    [SerializeField] private float maxThrowRange = 10f; // Max range throw
    [SerializeField] private Transform grabPoint;
    [SerializeField] private LayerMask groundLayer;

    [Header("Components")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;

    [SerializeField] Transform playerPosition;

    private Vector3 previousPlayerPosition;
    private Vector2 previousMousePosition;

    private float raycastInterval = 0.1f; // Frequency of raycasts (in seconds)
    private float timeSinceLastRaycast = 0f; // Count time since the last raycast


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void LateUpdate()
    {
        transform.position = playerPosition.position + new Vector3(0,-0.99f, 0);

        // Check if the mouse or player has moved
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        Vector3 currentPlayerPosition = playerPosition.position;

        if (currentMousePosition != previousMousePosition || currentPlayerPosition != previousPlayerPosition)
        {
            // Update time since the last raycast
            timeSinceLastRaycast += Time.deltaTime;

            // If the time since the last raycast is greater than the set interval
            if (timeSinceLastRaycast >= raycastInterval)
            {
                UpdateRangeIndicatorColor();
                previousMousePosition = currentMousePosition;
                previousPlayerPosition = currentPlayerPosition;
                timeSinceLastRaycast = 0f; // Reset time of last taycast
            }
        }
    }

    private void UpdateRangeIndicatorColor()
    {

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 250.0f, groundLayer))
        {
            Vector3 targetPoint = hit.point;
            float distance = Vector3.Distance(grabPoint.position, targetPoint);

            // If out of the range, turn red
            if (distance > maxThrowRange)
            {
                spriteRenderer.color = Color.red;
            }
            else // If in range, turn green
            {
                spriteRenderer.color = Color.green;
            }
        }
    }
    public void ActivateRangeCircle()
    {
        if ( animator != null)
        {
            gameObject.SetActive(true);
            animator.Play("RangeCircleAnim");
        }
    }

    public void DeactivateRangeCircle()
    {
        if (animator != null)
        {
            gameObject.SetActive(false);
        }
    }
}
