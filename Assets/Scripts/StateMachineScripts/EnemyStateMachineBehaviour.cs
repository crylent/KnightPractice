using Enemies;
using UnityEngine;

namespace StateMachineScripts
{
    public abstract class EnemyStateMachineBehaviour : LiveEntityStateMachineBehaviour
    {
        protected new static Enemy GetScriptComponent(Component component)
        {
            return component.GetComponent<Enemy>();
        }
    }
}
