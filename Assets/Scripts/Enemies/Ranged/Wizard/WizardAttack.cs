using System.Collections;
using UnityEngine;

namespace Enemies.Ranged.Wizard
{
   public class WizardAttack : RangedAttack
   {
      [Header("Wizard Attack Settings")]
      [SerializeField] private float attackDelay = 0.3f;

      private float _blockedUntil;
      private WizardMovement _wizardMovement;

      protected override void Awake()
      {
         base.Awake();
         _wizardMovement = GetComponent<WizardMovement>();
      }

      public void AddPostHitDelay(float delay)
      {
         _blockedUntil = Time.time + delay;
      }

      protected override bool CanAttack()
      {
         if (_wizardMovement && _wizardMovement.IsTeleporting) return false;
         return base.CanAttack() && Time.time >= _blockedUntil;
      }
   }
}