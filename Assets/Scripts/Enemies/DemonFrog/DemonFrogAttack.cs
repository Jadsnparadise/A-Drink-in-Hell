using UnityEngine;

namespace Enemies.DemonFrog
{
    public class DemonFrogAttack : EnemyAttack
    {
        private static readonly int Attack = Animator.StringToHash("Attack");
        private Animator _animator;

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponentInParent<Animator>();
        }

        protected override void PerformAttack()
        {
            base.PerformAttack();
            _animator.SetTrigger(Attack);
        }
    }
}