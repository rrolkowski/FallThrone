using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PlayerMovementController : MonoBehaviour
{
	private Rigidbody rb;

	private Vector2 movementInput;
	private Vector2 mousePosition;
	private Vector2 lastMousePosition;
	private Vector3 targetVelocity;

	[Header("Movement/Look Settings")]
	[SerializeField] float rotationSpeed = 20f;  // Prędkość rotacji gracza
	public float moveSpeed = 5f;
	[SerializeField] float acceleration = 20f;

	[Header("Srint Settings")]
	[SerializeField] float _sprintMultiplier = 1.5f;
	[SerializeField] float _sprintStaminaCost = 5f; //per second
	private bool isSprinting;

	[Header("Objects")]
	[SerializeField] Camera mainCamera;
	[SerializeField] Animator anim;

	[Header("References")]
	[SerializeField] StaminaSystem _staminaSystem;

	[Header("Animation Smoothing")]
	[Range(0, 1f)]
	public float HorizontalAnimSmoothTime = 0.2f;
	[Range(0, 1f)]
	public float VerticalAnimTime = 0.2f;
	[Range(0, 1f)]
	public float StartAnimTime = 0.3f;
	[Range(0, 1f)]
	public float StopAnimTime = 0.15f;

	private float speed;
	private float allowPlayerRotation = 0.1f;

    private bool _isGrounded;
    void Awake()
	{
		rb = GetComponent<Rigidbody>() ?? throw new MissingComponentException("Rigidbody is missing");
		mainCamera = mainCamera ?? Camera.main ?? throw new MissingReferenceException("Camera is missing");
		anim = anim ?? GetComponent<Animator>() ?? throw new MissingComponentException("Animator is missing");
	}

	void Update()
	{
		UpdateTargetVelocity();
		UpdateAnimation();  // Dodana obsługa animacji
	}

	void FixedUpdate()
	{
		HandleMovement();
		HandleRotation();  // Nowa funkcja do rotacji
	}

    public float GetRawMoveSpeed()
    {
        return moveSpeed;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    // Calculates how fast the player should go based on current input e.g from keyboard.
    void UpdateTargetVelocity()
	{
		Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y).normalized;

        float currentSpeed = moveSpeed;

        if (isSprinting && movement.magnitude > 0.1f)
        {
            if (_staminaSystem.ConsumeSprintStamina(_sprintStaminaCost))
            {
                currentSpeed *= _sprintMultiplier;
            }
            else
            {
                isSprinting = false;
            }
        }

        targetVelocity = movement * currentSpeed;

		//With Gravity v0.01
		if (!_isGrounded)
		{
			rb.linearVelocity += Physics.gravity * Time.fixedDeltaTime;
		}
	}

	// Changes player speed to make movement smooth
	void HandleMovement()
	{
        // Using Vector3.Lerp
        //rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // Movment with Gravity v0.01
        float currentYVelocity = rb.linearVelocity.y;
        Vector3 horizontalVelocity = Vector3.Lerp(new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z), targetVelocity, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(horizontalVelocity.x, currentYVelocity, horizontalVelocity.z);
    }

	// Rotates the player towards the direction of movement
	void HandleRotation()
	{
		// Sprawdzamy, czy gracz się porusza
		if (movementInput.sqrMagnitude > 0.01f)
		{
			// Wyznaczamy kąt, w którym gracz powinien się obrócić, zgodnie z kierunkiem ruchu
			Vector3 movementDirection = new Vector3(movementInput.x, 0, movementInput.y);
			Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

			// Interpolujemy rotację gracza w kierunku obliczonego kąta
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		}
	}

	// Input System Method for the "Move" action
	public void OnMove(InputAction.CallbackContext context)
	{
		movementInput = context.ReadValue<Vector2>();
	}
	
	public void OnSprint(InputAction.CallbackContext context)
	{
		isSprinting = context.ReadValueAsButton();
		Debug.Log("Sprint");
	}

	// Obsługa animacji
	void UpdateAnimation()
	{
		// Oblicz prędkość w oparciu o wektor ruchu
		speed = new Vector2(movementInput.x, movementInput.y).sqrMagnitude;

		// Animacja Blend - kontrola płynności animacji
		if (speed > allowPlayerRotation)
		{
			anim.SetFloat("Blend", speed, StartAnimTime, Time.deltaTime);
		}
		else
		{
			anim.SetFloat("Blend", speed, StopAnimTime, Time.deltaTime);
		}
	}

    //Gravity v0.01
    void OnCollisionStay(Collision collision)
    {
        // Sprawdza, czy gracz dotyka ziemi
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = false;
        }
    }
}
