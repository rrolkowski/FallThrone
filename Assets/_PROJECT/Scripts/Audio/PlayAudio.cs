using UnityEngine;

public class PlayAudio : MonoBehaviour
{
	[SerializeField] private SoundType sound;

	public void PlaySound()
	{
		AudioManager.PlaySound(sound);
	}
}
