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
	[SerializeField] float rotationSpeed = 20f;
	public float moveSpeed = 5f;
	[SerializeField] float acceleration = 20f;

	[Header("Srint Settings")]
	[SerializeField] float _sprintMultiplier = 1.5f;
	[SerializeField] float _sprintStaminaCost = 5f;
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
		UpdateAnimation();
	}

	void FixedUpdate()
	{
		HandleMovement();
		HandleRotation();
	}

	public float GetRawMoveSpeed()
	{
		return moveSpeed;
	}

	public void SetMoveSpeed(float newSpeed)
	{
		moveSpeed = newSpeed;
	}

	void UpdateTargetVelocity()
	{
		if (GameController.Instance != null && GameController.Instance.isTutorialActive)
		{
			targetVelocity = Vector3.zero;
			return;
		}

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

		if (!_isGrounded)
		{
			rb.linearVelocity += Physics.gravity * Time.fixedDeltaTime;
		}
	}

	void HandleMovement()
	{
		float currentYVelocity = rb.linearVelocity.y;
		Vector3 horizontalVelocity = Vector3.Lerp(new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z), targetVelocity, acceleration * Time.fixedDeltaTime);
		rb.linearVelocity = new Vector3(horizontalVelocity.x, currentYVelocity, horizontalVelocity.z);
	}

	void HandleRotation()
	{
		if (GameController.Instance != null && GameController.Instance.isTutorialActive)
			return;

		if (movementInput.sqrMagnitude > 0.01f)
		{
			Vector3 movementDirection = new Vector3(movementInput.x, 0, movementInput.y);
			Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		if (GameController.Instance != null && GameController.Instance.isTutorialActive)
		{
			movementInput = Vector2.zero;
			return;
		}

		movementInput = context.ReadValue<Vector2>();
	}

	public void OnSprint(InputAction.CallbackContext context)
	{
		if (GameController.Instance != null && GameController.Instance.isTutorialActive)
		{
			isSprinting = false;
			return;
		}

		isSprinting = context.ReadValueAsButton();
	}

	void UpdateAnimation()
	{
		speed = new Vector2(movementInput.x, movementInput.y).sqrMagnitude;

		if (speed > allowPlayerRotation)
		{
			anim.SetFloat("Blend", speed, StartAnimTime, Time.deltaTime);
		}
		else
		{
			anim.SetFloat("Blend", speed, StopAnimTime, Time.deltaTime);
		}
	}

	void OnCollisionStay(Collision collision)
	{
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
