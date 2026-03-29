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
        protected float LastAttackTime;

        protected virtual void Awake()
        {
            Movement = GetComponentInParent<EnemyMovement>();
            Animator = GetComponentInParent<Animator>();
            LastAttackTime = 0f;
        }

        protected virtual void Update()
        {
            var player = GetPlayerCollider();
            if (!player || !CanAttack()) return;
            if (Animator) Animator.SetTrigger(Attack);
            PerformAttack();
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
            return LastAttackTime + attackCooldown <= Time.time;
        }

        protected virtual void PerformAttack()
        {
            GameManager.Instance.DamagePlayer(damage);
            LastAttackTime = Time.time;
            if (runningAwayAfterAttacking)
                StartCoroutine(ActiveRunningAway());
        }

        protected virtual IEnumerator ActiveRunningAway()
        {
            yield return new WaitForSeconds(timeBeforeRunningAway);
            Movement.SetRunningAway();
        }
    }
}