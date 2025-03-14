using UnityEngine;
using System;

public enum SoundType
{
	MENU_Highlight_Button,
	MENU_Select_Button,
	MENU_Select_Play_Button,
	GAME_Player_Footsteps,
	GAME_Enemy_Footsteps,
	GAME_Enemy_Death,
	GAME_Enemy_Throw,
	GAME_Enemy_Grab,
	GAME_Enemy_Hit,
	MENU_Turret_Building,
	GAME_Turret_Fireball,
	GAME_Turret_Grab,
	GAME_Turret_Throw,
	GAME_Health,
	GAME_Lost,
	GAME_Win,
	GAME_Turret_Ice,
	GAME_Turret_AOE,
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class AudioManager : MonoBehaviour
{
	[SerializeField] private SoundList[] soundList;
	private static AudioManager Instance;
	private AudioSource audioSource;

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
	}

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public static void PlaySound(SoundType sound, float overrideVolume = -1)
	{
		SoundList selectedSoundList = Instance.soundList[(int)sound];
		AudioClip[] clips = selectedSoundList.Sounds;
		if (clips.Length == 0) return;

		AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

		float volume = overrideVolume >= 0 ? overrideVolume : selectedSoundList.Volume;

		Instance.audioSource.PlayOneShot(randomClip, volume);
	}

#if UNITY_EDITOR
	private void OnEnable()
	{
		string[] names = Enum.GetNames(typeof(SoundType));
		Array.Resize(ref soundList, names.Length);
		for (int i = 0; i < soundList.Length; i++)
			soundList[i].name = names[i];
	}
#endif
}

[Serializable]
public struct SoundList
{
	public AudioClip[] Sounds { get => sounds; }
	public float Volume { get => volume; }
	[HideInInspector] public string name;
	[Range(0, 1)][SerializeField] private float volume;
	[SerializeField] private AudioClip[] sounds;
}
