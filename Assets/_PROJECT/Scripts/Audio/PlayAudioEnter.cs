using UnityEngine;

public class PlayAudioEnter : StateMachineBehaviour
{
	[SerializeField] private SoundType sound;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		AudioManager.PlaySound(sound);
	}
}
