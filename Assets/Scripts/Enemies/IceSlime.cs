using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Enemies
{
    public class IceSlime: FlamySlime
    {
        [SerializeField] private ParticleSystem iceAreaEffect;
        [SerializeField] private float iceBlowRange = 30f;
        [SerializeField] private float iceAreaDistanceToCenter = 30f;
        [SerializeField] private float iceAreaLifetime = 15f;
        [SerializeField] private float iceBlowCooldown = 5f;

        private ActionCooldown _iceBlowCooldown;
            
        private static readonly int IceBlowTrigger = Animator.StringToHash("onBlow");

        protected override void Start()
        {
            base.Start();
            _iceBlowCooldown = new ActionCooldown(this, iceBlowCooldown);
        }

        protected override void AttackBehavior()
        {
            if (GetDistanceToPlayer() < iceBlowRange && _iceBlowCooldown.CanPerform)
            {
                Attack(IceBlowTrigger);
                _iceBlowCooldown.Cooldown();
            }
            else base.AttackBehavior();
        }

        private Vector3 _iceAreaCenter;

        public override void StartAttack(string attackName, ParticleSystem attackEffect)
        {
            if (!IsAlive) return;
            if (attackName == "Blow")
            {
                if (attackEffect.IsUnityNull()) return;
                var position = transform.position;
                var effectTransform = attackEffect!.transform;
                Instantiate(
                    attackEffect, 
                    position + effectTransform.position,
                    GetRotationToPlayer());
                _iceAreaCenter = position + GetDirectionToPlayer() * iceAreaDistanceToCenter;
            }
            else base.StartAttack(attackName, attackEffect);
        }

        public override void MakeDamage(string attackName, AttackCollider attackCollider)
        {
            if (attackName == "Blow")
            {
                Effects.PlayEffectForSeconds(iceAreaEffect, iceAreaLifetime, _iceAreaCenter);
            }
            else base.MakeDamage(attackName, attackCollider);
        }
    }
}