using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputManagerEntry;

public class PlayerMovementController : MonoBehaviour
{
    private Rigidbody rb;

    private Vector2 movementInput;
    private Vector2 mousePosition;
    private Vector2 lastMousePosition;
    private Vector3 targetVelocity;

    [Header("Movement/Look Settings")]
    [SerializeField] float rotationSpeed = 20f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float acceleration = 20f;

    [Header("Objects")]
    [SerializeField] Camera mainCamera;


    void Awake()
    {
        rb = GetComponent<Rigidbody>() ?? throw new MissingComponentException("Rigidbody is missing");
        mainCamera = mainCamera ?? Camera.main ?? throw new MissingReferenceException("Camera is missign");
    }

    void Update()
    {
        UpdateTargetVelocity();
        HandleLook();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    // Calculates how fast the player should go based on current input e.g from keyboard.
    void UpdateTargetVelocity()
    {
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        targetVelocity = movement * moveSpeed;
    }

    // Changes player speed to make movement smooth
    void HandleMovement()
    {
        //Using AddForce

        //Vector3 velocityChange = (targetVelocity - rb.linearVelocity) * acceleration * Time.fixedDeltaTime;
        //velocityChange.y = 0;
        //rb.AddForce(velocityChange, ForceMode.VelocityChange);


        // Using Vector3.Lerp
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    void HandleLook()
    {
        if (mousePosition == lastMousePosition)
        {
            return; // If the mouse did not moved, don't do a raycast
        }

        // Update the last mouse position
        lastMousePosition = mousePosition;

        // Create a raycast from the camera to where the mouse is on the screen
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        // Create a flat plane to connect cursor with 3D World
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayDistance;

        // Check if the ray hits the plane
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            // Get the exact point on the ground where the ray hits and find the directon from plaeyer to that point
            Vector3 targetPoint = ray.GetPoint(rayDistance);

            //Find the direction from the player to that point
            Vector3 directionToLook = (targetPoint - transform.position).normalized;

            // Set the rotation so that the player smoothly rotat towards that point
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToLook.x, 0f, directionToLook.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Draw a raycast
            Debug.DrawLine(ray.origin, targetPoint, Color.red);
        }

    }


    // Input System Method for the "Move" action
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    // Input System Method for the "Look" action
    public void OnLook(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }
}


