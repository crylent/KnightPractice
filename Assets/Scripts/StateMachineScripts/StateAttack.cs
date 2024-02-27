using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace StateMachineScripts
{
    public class StateAttack : LiveEntityStateMachineBehaviour
    {
        [SerializeField] private string attackName;
        [SerializeField] private AttackCollider attackCollider;
        [SerializeField] private float damageDelay;
        
        [SerializeField] [CanBeNull] private ParticleSystem attackEffect;
        [SerializeField] private bool attachEffect = true;
        
        [SerializeField] [CanBeNull] private AudioClip attackSound;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetScriptComponent(animator).StartAttack(attackName, attackEffect, attachEffect);
            var audio = GetAudioSource(animator);
            if (!audio.IsUnityNull() && !attackSound.IsUnityNull()) audio!.PlayOneShot(attackSound);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetScriptComponent(animator).MakeDamage(attackName, attackCollider, damageDelay);
        }
    }
}
