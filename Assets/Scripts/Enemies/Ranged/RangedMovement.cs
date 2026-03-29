using UnityEngine;

namespace Enemies.Ranged
{
    public class RangedMovement : EnemyMovement
    {
        protected Vector3 InitialSpawnPosition;

        // Idle Flip Settings
        protected float LastIdleFlipTime;
        protected float CurrentIdleFlipDelay;

        protected override void Awake()
        {
            base.Awake();
            InitialSpawnPosition = transform.position;
            ResetIdleFlipTimer();
        }

        protected override void Patrol()
        {
            if (IsOutsidePatrolArea())
            {
                ReturnToSpawn();
                return;
            }

            if (maxDistance <= 0.01f)
                HandleIdleFlipping();
            else
                base.Patrol();
        }

        protected bool IsOutsidePatrolArea()
        {
            var distToSpawn = Mathf.Abs(transform.position.x - InitialSpawnPosition.x);
            return distToSpawn > maxDistance + 0.05f;
        }

        protected void ReturnToSpawn()
        {
            ToRight = InitialSpawnPosition.x > transform.position.x;
            ApplyDirection();

            if (!RigidBody) return;

            var returnSpeed = speed > 0f ? speed : chasingSpeed;
            var direction = ToRight ? 1f : -1f;
            RigidBody.velocity = new Vector2(
                direction * returnSpeed, RigidBody.velocity.y
            );
        }

        protected void HandleIdleFlipping()
        {
            if (RigidBody)
                RigidBody.velocity = new Vector2(0f, RigidBody.velocity.y);

            if (Time.time > LastIdleFlipTime + CurrentIdleFlipDelay)
            {
                ToRight = !ToRight;
                ResetIdleFlipTimer();
            }

            ApplyDirection();
        }

        protected void ResetIdleFlipTimer()
        {
            LastIdleFlipTime = Time.time;
            CurrentIdleFlipDelay = Random.Range(2f, 5f);
        }

        protected void ApplyDirection()
        {
            var scaleX = transform.localScale.x;
            if ((ToRight && scaleX < 0f) || (!ToRight && scaleX > 0f))
            {
                Flip();
            }
        }

        protected override void Chasing()
        {
            UpdateChasingDirection();
            if (CanMoveInChasing())
                MoveBackward();
        }

        protected override bool CanMoveInChasing()
        {
            if (!PlayerSensor.Collider) return false;

            var playerPosition = PlayerSensor.Collider.transform.position;
            var playerDistance = Mathf.Abs(playerPosition.x - transform.position.x);

            var isTooClose = playerDistance < minDistanceToPlayer;
            var notAtLimit = ToRight
                ? transform.position.x > ChasingLeftLimit.x
                : transform.position.x < ChasingRightLimit.x;

            return isTooClose && notAtLimit;
        }

        protected virtual void MoveBackward()
        {
            if (!RigidBody) return;
            var direction = ToRight ? -1f : 1f;
            RigidBody.velocity = new Vector2(direction * chasingSpeed, RigidBody.velocity.y);
        }
    }
}
