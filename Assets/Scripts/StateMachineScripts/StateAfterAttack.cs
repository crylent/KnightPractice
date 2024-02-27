using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace StateMachineScripts
{
    public class StateAfterAttack : LiveEntityStateMachineBehaviour
    {
        [SerializeField] private ParticleSystem afterAttackEffect;
        [SerializeField] private bool attachEffect = true;

        [SerializeField] [CanBeNull] private AudioClip afterAttackSound;
        
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!afterAttackSound.IsUnityNull()) GetAudioSource(animator)?.PlayOneShot(afterAttackSound);
            GetScriptComponent(animator).AfterAttack(afterAttackEffect, attachEffect);
        }
    }
}
