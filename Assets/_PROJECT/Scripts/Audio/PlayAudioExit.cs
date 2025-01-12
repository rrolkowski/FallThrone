using UnityEngine;

public class PlayAudioExit : StateMachineBehaviour
{
	[SerializeField] private SoundType sound;

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		AudioManager.PlaySound(sound);
	}
}
