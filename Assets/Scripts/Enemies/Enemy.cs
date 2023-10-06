using System;
using System.Collections;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : LiveEntity
    {
        [SerializeField] protected GameObject threatArrow;
        
        protected Vector3 Movement = Vector3.zero;

        [NonSerialized] public bool BehaviorEnabled;

        protected override void Update()
        {
            base.Update();
            if (BehaviorEnabled) BehaviorUpdate();
            _posDiff = PlayerComponents.Transform.position - gameObject.transform.position; // positions difference
            _posDiff.y = 0; // don't consider the height
        }

        protected abstract void BehaviorUpdate();

        protected void Attack(int triggerId)
        {
            if (IsAttacking) return;
            
            IsAttacking = true;
            Animator.SetTrigger(triggerId);
        }

        protected override void MakeDamageOnTarget(AttackCollider hitbox)
        {
            if (hitbox.PlayerIsInside)
            {
                PlayerComponents.Controller.TakeDamage(this, hitbox.Damage);
            }
        }

        private Vector3 _posDiff; // relative player's position

        protected Vector3 GetDirectionToPlayer()
        {
            return _posDiff.normalized;
        }

        protected Quaternion GetRotationToPlayer()
        {
            var angle = Vector3.SignedAngle(_posDiff, Vector3.forward, Vector3.down);
            return Quaternion.Euler(90, angle, 0);
        }

        protected float GetDistanceToPlayer()
        {
            return _posDiff.magnitude;
        }

        public override void TakeDamage(LiveEntity producer = null, int damage = 1)
        {
            base.TakeDamage(producer, damage);
            if (!IsAlive) BehaviorEnabled = false; // no more attacks from the grave
        }
        
        protected class ActionCooldown
        {
            private bool _onCooldown;
            public bool CanPerform => !_onCooldown;
            private readonly MonoBehaviour _owner;
            private readonly float _cooldownTime;

            public ActionCooldown(MonoBehaviour owner, float cooldownTime)
            {
                _owner = owner;
                _cooldownTime = cooldownTime;
            }

            public void Cooldown()
            {
                _owner.StartCoroutine(CooldownCoroutine());
            }

            private IEnumerator CooldownCoroutine()
            {
                _onCooldown = true;
                yield return new WaitForSeconds(_cooldownTime);
                _onCooldown = false;
            }
        }
    }
}
