using UnityEngine;

namespace StateMachineScripts
{
    public class StateAttack : LiveEntityStateMachineBehaviour
    {
        [SerializeField] private AttackCollider attackCollider;
        
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetScriptComponent(animator).StartAttack(attackCollider);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetScriptComponent(animator).MakeDamage(attackCollider);
        }
    }
}
