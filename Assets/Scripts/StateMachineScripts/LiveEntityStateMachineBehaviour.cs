using UnityEngine;

namespace StateMachineScripts
{
    public abstract class LiveEntityStateMachineBehaviour : StateMachineBehaviour
    {
        protected static LiveEntity GetScriptComponent(Component component)
        {
            return component.GetComponent<LiveEntity>();
        }
    }
}
