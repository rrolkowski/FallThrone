using UnityEngine;

public class Billboard : MonoBehaviour
{
	public Camera cam;

	private void Start()
	{
		// ZnajdŸ kamerê na podstawie przypisanego tagu
		cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		if (cam != null)
		{
			transform.LookAt(transform.position + cam.transform.forward);
		}
	}
}
