using UnityEngine;

public class AmbientSoundController : MonoBehaviour
{
	private AudioSource audioSource;

	private bool wasPaused = false;

	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
			Debug.LogError("[AmbientSoundController] Brak AudioSource na obiekcie!");
	}

	void Update()
	{
		if (GameState.STATE_Won || GameState.STATE_Lost)
		{
			if (audioSource.isPlaying)
				audioSource.Stop(); // Ca³kowite wy³¹czenie
			return;
		}

		if (GameState.STATE_Paused)
		{
			if (audioSource.isPlaying)
			{
				audioSource.Pause();
				wasPaused = true;
			}
		}
		else
		{
			if (!audioSource.isPlaying && wasPaused)
			{
				audioSource.UnPause();
				wasPaused = false;
			}
		}
	}
}
