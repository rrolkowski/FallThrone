using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AmbientSoundManager : MonoBehaviour
{
	private AudioSource audioSource;

	[Header("Ambient Settings")]
	public AudioClip[] ambientClips;
	public float minDelay = 5f;
	public float maxDelay = 20f;
    public AudioMixerGroup mixerGroup;

    [Range(0f, 1f)]
	public float volume = 1f; // Dodane ustawienie głośności

	void Start()
	{
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.loop = false;
		audioSource.playOnAwake = false;
		audioSource.volume = volume;

		StartCoroutine(PlayAmbientLoop());
	}

	IEnumerator PlayAmbientLoop()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

			if (ambientClips.Length > 0)
			{
				AudioClip clip = ambientClips[Random.Range(0, ambientClips.Length)];
				audioSource.volume = volume; // Ustawienie aktualnej głośności
				audioSource.PlayOneShot(clip);
			}
		}
	}

	public void SetVolume(float newVolume)
	{
		volume = Mathf.Clamp01(newVolume);
	}
}
