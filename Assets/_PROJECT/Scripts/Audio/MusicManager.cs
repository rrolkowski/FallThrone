using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class MusicManager : MonoBehaviour
{
	private static MusicManager Instance;
	private AudioSource musicSource;
	private AudioLowPassFilter lowPassFilter;

	[Header("Music Clips")]
	[SerializeField] private AudioClip menuMusic;
	[SerializeField] private AudioClip gameMusic_level1;
	[SerializeField] private AudioClip gameMusic_level2;
	[SerializeField] private AudioClip gameMusic_level3;
	[SerializeField] private AudioClip gameMusic_level4;
	[SerializeField] private AudioClip gameMusic_level5;
	[SerializeField] private AudioClip gameMusic_level6;
	[SerializeField] private AudioClip gameMusic_level7;
	[SerializeField] private AudioClip gameMusic_level8;
	[SerializeField] private AudioClip gameMusic_level9;

	[Header("Volume Settings")]
	[Range(0, 1)][SerializeField] private float menuMusicVolume = 0.5f;
	[Range(0, 1)][SerializeField] private float gameMusicVolume = 0.5f;

	private void Awake()
	{
		if (!Application.isPlaying) return;

		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		musicSource = GetComponent<AudioSource>();
		lowPassFilter = GetComponent<AudioLowPassFilter>();

		musicSource.loop = true;
		lowPassFilter.cutoffFrequency = 22000f;
	}

	private void Update()
	{
		if (musicSource == null || lowPassFilter == null) return;

		// Apply effect if Paused OR Shop
		if (GameState.STATE_Paused || GameState.STATE_Shop)
		{
			musicSource.volume = GetTargetVolume() * 0.6f;
			musicSource.pitch = 0.95f; // 5% wolniej
			lowPassFilter.cutoffFrequency = 500f;
		}
		else
		{
			musicSource.volume = GetTargetVolume();
			musicSource.pitch = 1.0f;
			lowPassFilter.cutoffFrequency = 22000f;
		}
	}

	private float GetTargetVolume()
	{
		return musicSource.clip == menuMusic ? menuMusicVolume : gameMusicVolume;
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
		else if (scene.name.StartsWith("Level_"))
		{
			if (int.TryParse(scene.name.Replace("Level_", ""), out int level))
			{
				PlayGameMusic(level);
			}
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

		AudioClip selected = level switch
		{
			1 => Instance.gameMusic_level1,
			2 => Instance.gameMusic_level2,
			3 => Instance.gameMusic_level3,
			4 => Instance.gameMusic_level4,
			5 => Instance.gameMusic_level5,
			6 => Instance.gameMusic_level6,
			7 => Instance.gameMusic_level7,
			8 => Instance.gameMusic_level8,
			9 => Instance.gameMusic_level9,
			_ => null
		};

		if (selected != null)
			Instance.StartCoroutine(Instance.PlayMusicWithFade(selected, Instance.gameMusicVolume, 3));
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
			musicSource.volume = GetTargetVolume();
		}
	}
#endif
}
