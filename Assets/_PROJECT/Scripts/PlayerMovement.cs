using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
	public static PlayerMovement Instance {  get; private set; }

	[HideInInspector] public NavMeshAgent agent;
	public Camera cam;

	private float walkSpeed = 4f;
	private float runSpeed = 7f;

	private void Awake()
	{
		Instance = this;

		agent = GetComponent<NavMeshAgent>();
		agent.acceleration = 50f;
		agent.stoppingDistance = 0.01f;
		agent.angularSpeed = 720f;
		agent.autoBraking = false;
	}

	private void Start()
	{
		agent.speed = walkSpeed;
	}

	private void Update()
	{
		Movement();
	}

	//

	public void Move(RaycastHit hit)
	{
		agent.destination = hit.point;
	}

	public void Stop()
	{
		agent.ResetPath();
	}

	void Movement()
	{
		if (Input.GetMouseButton(1))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
			{
				Move(hit);
			}
		}
		else
		{
			Stop();
		}

		if (Input.GetKey(KeyCode.LeftShift))
		{
			agent.speed = runSpeed;
		}
		else
		{
			agent.speed = walkSpeed;
		}
	}
}
