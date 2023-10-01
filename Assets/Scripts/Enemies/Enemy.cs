using System;
using System.Collections;
using Player;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : LiveEntity
    {
        protected Vector3 Movement = Vector3.zero;

        private OpacityController.OpacityController _opacityController;

        [NonSerialized] public bool BehaviorEnabled = false;

        protected void Update()
        {
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

        public override void MakeDamage(AttackCollider attackCollider)
        {
            StartCoroutine(MakeDamageCoroutine(attackCollider));
        }

        private IEnumerator MakeDamageCoroutine(AttackCollider attackCollider)
        {
            var hitbox = Instantiate(attackCollider, gameObject.transform);
            hitbox.transform.parent = null; // detach from parent to ignore other attack colliders
            yield return new WaitForFixedUpdate(); // wait for OnTriggerEnter execution
            if (hitbox.PlayerIsInside)
            {
                PlayerComponents.Controller.TakeDamage(this, hitbox.Damage);
            }
            Destroy(hitbox.gameObject);
        }

        private Vector3 _posDiff; // relative player's position

        protected Vector3 GetDirectionToPlayer()
        {
            return _posDiff.normalized;
        }

        protected float GetDistanceToPlayer()
        {
            return _posDiff.magnitude;
        }

        public override void TakeDamage(LiveEntity producer = null, int damage = 1)
        {
            base.TakeDamage(producer, damage);
            BehaviorEnabled = false; // no more attacks from the grave
        }
    }
}
