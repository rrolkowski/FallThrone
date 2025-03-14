using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
	private static MusicManager Instance;
	private AudioSource musicSource;

	[Header("Music Clips")]
	[SerializeField] private AudioClip menuMusic;
	[SerializeField] private AudioClip gameMusic_level1;
	[SerializeField] private AudioClip gameMusic_level2;
	[SerializeField] private AudioClip gameMusic_level3;

	[Header("Volume Settings")]
	[Range(0, 1)]
	[SerializeField] private float menuMusicVolume = 0.5f; // G³oœnoœæ dla menu
	[Range(0, 1)]
	[SerializeField] private float gameMusicVolume = 0.5f; // G³oœnoœæ dla gry

	private void Awake()
	{
		if (!Application.isPlaying)
			return;

		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		musicSource = GetComponent<AudioSource>();
		musicSource.loop = true; // Ustawienie pêtli na true
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "Menu")
		{
			PlayMenuMusic();
		}
		else if (scene.name == "Level_1")
		{
			PlayGameMusic(1);
		}
		else if (scene.name == "Level_2")
		{
			PlayGameMusic(2);
		}
		else if (scene.name == "Level_3")
		{
			PlayGameMusic(3);
		}
	}

	public static void PlayMenuMusic()
	{
		if (Instance == null || Instance.menuMusic == null) return;

		Instance.StartCoroutine(Instance.PlayMusicWithFade(Instance.menuMusic, Instance.menuMusicVolume, 1));
	}

	public static void PlayGameMusic(int level)
	{
		if (Instance == null) return;

		if (level == 1)
			Instance.StartCoroutine(Instance.PlayMusicWithFade(Instance.gameMusic_level1, Instance.gameMusicVolume, 3));
		else if (level == 2)
			Instance.StartCoroutine(Instance.PlayMusicWithFade(Instance.gameMusic_level2, Instance.gameMusicVolume, 3));
		else if (level == 3)
			Instance.StartCoroutine(Instance.PlayMusicWithFade(Instance.gameMusic_level3, Instance.gameMusicVolume, 3));
	}

	public static void StopMusic()
	{
		if (Instance == null) return;

		Instance.musicSource.Stop();
	}

	public static void SetVolume(float volume)
	{
		if (Instance == null) return;

		Instance.musicSource.volume = Mathf.Clamp01(volume);
	}

	private IEnumerator PlayMusicWithFade(AudioClip clip, float targetVolume, float fadeDuration = 1f)
	{
		if (clip == null) yield break;

		// Stop current music
		musicSource.Stop();
		musicSource.clip = clip;
		musicSource.volume = 0;
		musicSource.Play();

		float elapsed = 0f;

		while (elapsed < fadeDuration)
		{
			elapsed += Time.deltaTime;
			musicSource.volume = Mathf.Lerp(0, targetVolume, elapsed / fadeDuration);
			yield return null;
		}

		musicSource.volume = targetVolume;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (musicSource != null)
		{
			if (musicSource.clip == menuMusic)
				musicSource.volume = menuMusicVolume;
			else if (musicSource.clip == gameMusic_level1)
				musicSource.volume = gameMusicVolume;
			else if (musicSource.clip == gameMusic_level2)
				musicSource.volume = gameMusicVolume;
			else if (musicSource.clip == gameMusic_level3)
				musicSource.volume = gameMusicVolume;
		}
	}
#endif
}
