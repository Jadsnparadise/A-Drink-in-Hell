using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemies
{
    public class EnemyAttack : MonoBehaviour
    {
        protected static readonly int Attack = Animator.StringToHash("Attack");

        [Header("Stats")]
        [SerializeField] protected int damage;
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected bool runningAwayAfterAttacking = false;
        [SerializeField] protected float timeBeforeRunningAway = 0.2f;

        [Header("References")]
        public Collider2D hitboxCollider;

        protected EnemyMovement Movement;
        protected Animator Animator;
        protected EnemyController Controller;
        protected float LastInitialAttackTime;
        protected float LastAttackTime;
        protected bool Attacking = false;

        protected virtual void Awake()
        {
            Movement = GetComponentInParent<EnemyMovement>();
            Animator = GetComponentInParent<Animator>();
            Controller = GetComponentInParent<EnemyController>();
            LastAttackTime = 0f;
        }

        protected virtual void Update()
        {
            if (Controller.IsDead()) return;

            var player = GetPlayerCollider();
            if (!player || !CanAttack() || !Animator) return;

            LastInitialAttackTime = Time.time;
            Attacking = true;
            Animator.SetTrigger(Attack);
        }

        protected virtual Collider2D GetPlayerCollider()
        {
            List<Collider2D> colliders = new();
            var filter = new ContactFilter2D() { useTriggers = false };
            Physics2D.OverlapCollider(hitboxCollider, filter, colliders);

            return colliders.FirstOrDefault(col => col.CompareTag("Player"));
        }

        protected virtual bool CanAttack()
        {
            if (LastInitialAttackTime + attackCooldown <= Time.time)
                Attacking = false;

            return LastAttackTime + attackCooldown <= Time.time && !Attacking;
        }

        protected virtual void PerformAttack()
        {
            LastAttackTime = Time.time;

            var player = GetPlayerCollider();
            if (!player || Controller.IsDead()) return;

            GameManager.Instance.DamagePlayer(damage);
            if (runningAwayAfterAttacking)
                StartCoroutine(ActiveRunningAway());
        }

        protected virtual void OnAttackAnimationEnd()
        {
            Attacking = false;
        }

        protected virtual IEnumerator ActiveRunningAway()
        {
            yield return new WaitForSeconds(timeBeforeRunningAway);
            Movement.SetRunningAway();
        }
    }
}