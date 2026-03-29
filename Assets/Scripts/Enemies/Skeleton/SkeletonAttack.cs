using System.Collections;
using UnityEngine;

namespace Enemies.Skeleton
{
    public sealed class SkeletonAttack : EnemyAttack
    {
        private static readonly WaitForSeconds WaitForSeconds03 = new WaitForSeconds(0.3f);

        protected override void Update()
        {
            var player = GetPlayerCollider();
            if (!player || !CanAttack()) return;
            
            LastAttackTime = Time.time; 
            StartCoroutine(PerformAttackCoroutine());
        }

        private IEnumerator PerformAttackCoroutine()
        {
            if (Animator) Animator.SetTrigger(Attack);
            yield return WaitForSeconds03;
            PerformAttack();
        }
        
        protected override void PerformAttack()
        {
            GameManager.Instance.DamagePlayer(damage);
            if (runningAwayAfterAttacking)
                StartCoroutine(ActiveRunningAway());
        }
    }
}