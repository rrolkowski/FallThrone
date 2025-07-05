using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioSourceScanner : MonoBehaviour
{
	private static AudioSourceScanner instance;
	public float scanInterval = 2f;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject); // unikaj duplikatów
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);
		StartCoroutine(ScanLoop());

		Debug.Log("[AudioSourceScanner] Started.");
	}

	IEnumerator ScanLoop()
	{
		while (true)
		{
			ScanForAudioSources();
			yield return new WaitForSeconds(scanInterval);
		}
	}

    void ScanForAudioSources()
	{
		AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None); // true = tak¿e nieaktywne
		foreach (var src in sources)
		{
            if (src.isPlaying)
            {
                string clipName = src.clip != null ? src.clip.name : "NULL (PlayOneShot?)";
                Debug.Log($"[AUDIO SCANNER] {src.clip.name} is playing on {src.gameObject.name}", src.gameObject);
			}
		}
	}
}
