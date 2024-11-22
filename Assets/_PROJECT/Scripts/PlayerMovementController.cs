using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
	private Rigidbody rb;

	private Vector2 movementInput;
	private Vector2 mousePosition;
	private Vector2 lastMousePosition;
	private Vector3 targetVelocity;

	[Header("Movement/Look Settings")]
	[SerializeField] float rotationSpeed = 20f;  // Prędkość rotacji gracza
	[SerializeField] float moveSpeed = 5f;
	[SerializeField] float acceleration = 20f;

	[Header("Objects")]
	[SerializeField] Camera mainCamera;
	[SerializeField] Animator anim;

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

	// Calculates how fast the player should go based on current input e.g from keyboard.
	void UpdateTargetVelocity()
	{
		Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y).normalized;
		targetVelocity = movement * moveSpeed;

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
