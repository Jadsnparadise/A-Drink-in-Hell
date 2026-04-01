using UnityEngine;

namespace Enemies.DemonFrog
{
    public sealed class DemonFrogMovement : EnemyMovement
    {
        [Header("Jump configurations")]
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float jumpProbability;

        private float _lastJumpTime;

        protected override void Awake()
        {
            _lastJumpTime = 0f;
            base.Awake();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Jump();
        }

        public override void Jump()
        {
            if (Controller.IsDead()) return;
            if (Time.time < _lastJumpTime + jumpCooldown) return;
            var random = Random.Range(0, jumpProbability);
            if (!(random <= jumpProbability)) return;
            base.Jump();
            _lastJumpTime = Time.time;
        }
    }
}