using UnityEngine;
using UnityEngine.Serialization;

namespace PowerUps
{
    public abstract class PowerUp: MonoBehaviour
    {
        public string powerUpName;
        public string powerUpDesc;
        [FormerlySerializedAs("unique")] public bool isUnique; // can be applied once only
        
        public abstract void ApplyEffect();
        public virtual void CancelEffect() {}
    }
}