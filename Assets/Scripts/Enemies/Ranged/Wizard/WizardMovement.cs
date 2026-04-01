using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Enemies.Ranged.Wizard
{
    public sealed class WizardMovement : RangedMovement
    {
        private static readonly WaitForSeconds WaitForSeconds02 = new WaitForSeconds(0.2f);
        private static readonly WaitForSeconds WaitForSeconds1 = new WaitForSeconds(1f);

        [Header("Teleport Settings")]
        [SerializeField] private float teleportCooldown = 5f;
        [SerializeField] private float teleportRadius = 5f;
        [SerializeField] private LayerMask obstacleLayer;

        protected float LastTeleportTime;
        public bool IsTeleporting { get; protected set; }
        protected Vector2 TeleportTargetPoint;
        protected WizardAttack WizardAttack;
        protected Collider2D Collider;
        protected SpriteRenderer SpriteRenderer;

        protected static readonly int DisappearTrigger = Animator.StringToHash("Disappear");
        protected static readonly int ReappearTrigger = Animator.StringToHash("Reappear");

        protected override void Awake()
        {
            base.Awake();
            LastTeleportTime = -teleportCooldown;

            InitializeColliders();

            Controller = GetComponentInChildren<EnemyController>();
            if (Controller)
                Controller.OnDamaged += HandleDamageTaken;
            WizardAttack = GetComponent<WizardAttack>();
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected void OnDestroy()
        {
            if (Controller)
                Controller.OnDamaged -= HandleDamageTaken;
        }

        protected void HandleDamageTaken(int amount)
        {
            if (Controller && Controller.IsDead())
            {
                if (RigidBody) RigidBody.gravityScale = 1f;
                return;
            }

            if (IsTeleporting) return;

            if (!PlayerSensor.Collider) return;

            var playerX = PlayerSensor.Collider.transform.position.x;
            var myX = transform.position.x;
            var playerToRight = playerX > myX;

            if (ToRight == playerToRight) return;

            ToRight = playerToRight;
            Flip();
            if (WizardAttack)
                WizardAttack.AddPostHitDelay(0.3f);
        }

        protected void InitializeColliders()
        {
            Collider = GetComponentInParent<Collider2D>();
        }

        protected override void FixedUpdate()
        {
            if (IsTeleporting)
            {
                if (Time.time > LastTeleportTime + 3f)
                    CompleteTeleportReappear();

                StopMovement();
                if (Animator) Animator.SetBool(IsWalking, false);
                return;
            }
            base.FixedUpdate();
        }

        protected override void Chasing()
        {
            if (IsTeleporting) return;

            UpdateChasingDirection();

            if (!PlayerSensor.Collider) return;

            if (IsPlayerTooClose())
                HandleEvasion();
        }

        protected bool IsPlayerTooClose()
        {
            var playerPosition = PlayerSensor.Collider.transform.position;
            var playerDist = Mathf.Abs(playerPosition.x - transform.position.x);
            return playerDist < minDistanceToPlayer;
        }

        protected override bool CanMoveInChasing()
        {
            if (!PlayerSensor.Collider) return false;
            return IsPlayerTooClose() && !IsWallBehind();
        }

        protected void HandleEvasion()
        {
            if (CanTeleport() && TryStartTeleport(IsCornered()))
                return;

            if (CanMoveInChasing())
                MoveBackward();
            else
                StopMovement();
        }

        protected bool CanTeleport()
        {
            return Time.time >= LastTeleportTime + teleportCooldown;
        }

        protected bool IsCornered()
        {
            return !CanMoveInChasing() || IsWallBehind();
        }

        protected void StopMovement()
        {
            if (RigidBody)
                RigidBody.velocity = new Vector2(0f, RigidBody.velocity.y);
        }

        protected bool IsWallBehind()
        {
            if (!Collider) return false;

            var direction = ToRight ? -1f : 1f;
            Vector2 origin = Collider.bounds.center;
            var distance = Collider.bounds.extents.x + 0.2f;

            var hits = Physics2D.RaycastAll(origin, new Vector2(direction, 0), distance);
            return hits.Any(hit => !hit.collider.isTrigger && IsObstacle(hit.collider));
        }

        protected bool TryStartTeleport(bool isCornered)
        {
            if (!TryFindBestTeleportTarget(out var bestPoint, out var bestDist))
                return false;

            var playerPos = PlayerSensor.Collider.transform.position;
            var currentDist = Vector2.Distance(transform.position, playerPos);

            if (!(bestDist > currentDist) && !isCornered) return false;
            ExecuteTeleportSequence(bestPoint);
            return true;
        }

        protected bool TryFindBestTeleportTarget(out Vector2 bestPoint, out float bestDist)
        {
            var playerPos = PlayerSensor.Collider.transform.position;
            var radiusForCheck = Collider ? Collider.bounds.extents.x : 0.5f;

            bestPoint = Vector2.zero;
            bestDist = -1f;
            var foundTarget = false;

            for (var i = 0; i < 3; i++)
            {
                var randomX = Random.Range(-teleportRadius, teleportRadius);
                var candidate = new Vector2(InitialSpawnPosition.x + randomX, InitialSpawnPosition.y);

                if (!IsPositionClear(candidate, radiusForCheck)) continue;

                var distFromPlayer = Vector2.Distance(candidate, playerPos);
                if (!(distFromPlayer > bestDist)) continue;

                bestDist = distFromPlayer;
                bestPoint = candidate;
                foundTarget = true;
            }

            return foundTarget;
        }

        protected bool IsPositionClear(Vector2 position, float checkRadius)
        {
            var colliders = Physics2D.OverlapCircleAll(position, checkRadius);
            return !colliders.Any(col => !col.isTrigger && IsObstacle(col));
        }

        protected bool IsObstacle(Collider2D col)
        {
            return col.TryGetComponent<TilemapCollider2D>(out _) ||
                   ((obstacleLayer.value & (1 << col.gameObject.layer)) > 0);
        }

        protected void ExecuteTeleportSequence(Vector2 targetPoint)
        {
            TeleportTargetPoint = targetPoint;
            IsTeleporting = true;
            LastTeleportTime = Time.time;

            if (Collider) Collider.enabled = false;
            if (RigidBody)
            {
                RigidBody.gravityScale = 0f;
                RigidBody.velocity = Vector2.zero;
            }

            StopMovement();

            if (Animator) Animator.SetTrigger(DisappearTrigger);
        }

        protected void CompleteTeleportDisappear()
        {
            transform.position = TeleportTargetPoint;

            ChasingRightLimit = transform.position;
            ChasingRightLimit.x += chasingMaxDistance;
            ChasingLeftLimit = transform.position;
            ChasingLeftLimit.x -= chasingMaxDistance;

            if (PlayerSensor.Collider)
            {
                var shouldFaceRight = PlayerSensor.Collider.transform.position.x > transform.position.x;
                ToRight = shouldFaceRight;
                var scaleX = transform.localScale.x;
                if ((ToRight && scaleX < 0f) || (!ToRight && scaleX > 0f))
                    Flip();
            }

            if (Animator) Animator.SetTrigger(ReappearTrigger);
        }

        protected void CompleteTeleportReappear()
        {
            if (Collider) Collider.enabled = true;
            if (RigidBody) RigidBody.gravityScale = 1f;

            IsTeleporting = false;
        }
    }
}