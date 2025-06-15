using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
	public int fpsLimit = 60;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		// Ustaw limit FPS do 120
		Application.targetFrameRate = fpsLimit;
	}

	// Update is called once per frame
	void Update()
	{

	}
}
