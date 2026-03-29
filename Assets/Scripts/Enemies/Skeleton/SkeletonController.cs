using System.Collections;
using UnityEngine;

namespace Enemies.Skeleton
{
    public sealed class SkeletonController : EnemyController
    {
        private static readonly WaitForSeconds WaitForSeconds1 = new WaitForSeconds(1f);
        private static readonly int Death = Animator.StringToHash("Death");
        private Animator _animator;
        
        protected override void Awake()
        {
            _animator = GetComponentInParent<Animator>();
            base.Awake();
        }
        
        protected override IEnumerator DieRoutine()
        {
            _animator.SetTrigger(Death);
            yield return WaitForSeconds1;
            Destroy(gameObject);
        }
    }
}