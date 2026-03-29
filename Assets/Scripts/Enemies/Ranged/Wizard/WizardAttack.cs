using System.Collections;
using UnityEngine;

namespace Enemies.Ranged.Wizard
{
   public class WizardAttack : RangedAttack
   {
      [Header("Wizard Attack Settings")]
      [SerializeField] private float attackDelay = 0.3f;

      private float _blockedUntil;

      public void AddPostHitDelay(float delay)
      {
         _blockedUntil = Time.time + delay;
      }

      protected override bool CanAttack()
      {
         return base.CanAttack() && Time.time >= _blockedUntil;
      }

      protected override void PerformAttack(Collider2D player)
      {
         StartCoroutine(DelayedPerformAttack(player));
      }

      private IEnumerator DelayedPerformAttack(Collider2D player)
      {
         yield return new WaitForSeconds(attackDelay);
         if (player)
            base.PerformAttack(player);
      }
   }
}