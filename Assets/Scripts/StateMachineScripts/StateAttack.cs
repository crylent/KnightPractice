using UnityEngine;

namespace StateMachineScripts
{
    public class StateAttack : LiveEntityStateMachineBehaviour
    {
        [SerializeField] private string attackName;
        [SerializeField] private AttackCollider attackCollider;
        [SerializeField] private ParticleSystem attackEffect;
        
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetScriptComponent(animator).StartAttack(attackName, attackCollider, attackEffect);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetScriptComponent(animator).MakeDamage(attackName, attackCollider);
        }
    }
}
