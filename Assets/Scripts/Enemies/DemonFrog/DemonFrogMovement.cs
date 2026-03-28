using UnityEngine;

namespace Enemies.DemonFrog
{
    public sealed class DemonFrogMovement : EnemyMovement
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");

        [Header("Jump configurations")]
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float jumpProbability;
        
        private Animator _animator;
        private float _lastJumpTime;

        protected override void Awake()
        {
            _animator = GetComponentInParent<Animator>();
            _lastJumpTime = 0f;
            base.Awake();
        }

        private void Update()
        {
            UpdateAnimation();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Jump();
        }

        public override void Jump()
        {
            if (Time.time < _lastJumpTime + jumpCooldown) return;
            var random = Random.Range(0, jumpProbability);
            if (!(random <= jumpProbability)) return;
            base.Jump();
            _lastJumpTime =  Time.time;
        }

        private void UpdateAnimation()
        {
            if (!_animator || !RigidBody) return;

            var isWalking = Mathf.Abs(RigidBody.velocity.x) > 0.1f;
            
            _animator.SetBool(IsWalking, isWalking);
            _animator.SetBool(IsJumping, !IsGrounded);
        }
    }
}