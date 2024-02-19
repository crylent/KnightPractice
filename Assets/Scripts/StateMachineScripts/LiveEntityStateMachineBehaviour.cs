using JetBrains.Annotations;
using UnityEngine;

namespace StateMachineScripts
{
    public abstract class LiveEntityStateMachineBehaviour : StateMachineBehaviour
    {
        protected static LiveEntity GetScriptComponent(Component component)
        {
            return component.GetComponent<LiveEntity>();
        }

        [CanBeNull]
        protected static AudioSource GetAudioSource(Component component)
        {
            return component.GetComponent<AudioSource>();
        }
    }
}
