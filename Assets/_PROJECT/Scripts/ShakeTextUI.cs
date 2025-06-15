using UnityEngine;

public class ShakeTextUI : MonoBehaviour
{
	[Header("Pulsowanie Skali")]
	[Range(0f, 0.5f)] public float scaleAmplitude = 0.1f; // jak mocno skala siê zmienia
	[Range(0.1f, 10f)] public float scaleFrequency = 2f;   // jak szybko skala pulsuje

	[Header("Rotacja Z")]
	[Range(0f, 10f)] public float rotationAmplitude = 5f;  // maksymalny k¹t w stopniach
	[Range(0.1f, 10f)] public float rotationFrequency = 3f; // jak szybko rotacja siê zmienia

	private Vector3 initialScale;
	private Quaternion initialRotation;

	private float randomScaleOffset;
	private float randomRotationOffset;

	void Start()
	{
		initialScale = transform.localScale;
		initialRotation = transform.localRotation;

		// Losowe offsety fazy, ¿eby efekt nie by³ sztywny
		randomScaleOffset = Random.Range(0f, 100f);
		randomRotationOffset = Random.Range(0f, 100f);
	}

	void Update()
	{
		// Oblicz now¹ skalê
		float scaleOscillation = 1f + Mathf.Sin(Time.time * scaleFrequency + randomScaleOffset) * scaleAmplitude;
		transform.localScale = initialScale * scaleOscillation;

		// Oblicz now¹ rotacjê Z
		float rotationZ = Mathf.Sin(Time.time * rotationFrequency + randomRotationOffset) * rotationAmplitude;
		transform.localRotation = initialRotation * Quaternion.Euler(0f, 0f, rotationZ);
	}
}
