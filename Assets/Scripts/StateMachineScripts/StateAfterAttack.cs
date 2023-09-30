using UnityEngine;

namespace StateMachineScripts
{
    public class StateAfterAttack : LiveEntityStateMachineBehaviour
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetScriptComponent(animator).AfterAttack();
        }
    }
}
