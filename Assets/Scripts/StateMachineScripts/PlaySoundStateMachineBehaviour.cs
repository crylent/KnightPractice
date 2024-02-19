using UnityEngine;

namespace StateMachineScripts
{
    public class PlaySoundStateMachineBehaviour: LiveEntityStateMachineBehaviour
    {
        [SerializeField] private AudioClip sound;
        [SerializeField] private bool playOnExit;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if (!playOnExit) GetAudioSource(animator)?.PlayOneShot(sound);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            if (playOnExit) GetAudioSource(animator)?.PlayOneShot(sound);
        }
    }
}