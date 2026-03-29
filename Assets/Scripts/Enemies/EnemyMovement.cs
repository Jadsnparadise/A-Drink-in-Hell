using UnityEngine;

namespace Enemies
{
    public class EnemyMovement : MonoBehaviour
    {
        protected enum MovementState
        {
            Patrolling,
            Chasing,
            RunningAway
        }
        
        [Header("Movement Settings")]
        [SerializeField] private float minDistanceToPlayer;
        
        [Header("Patrol Stats")]
        [SerializeField] private float speed;
        [SerializeField] private float maxDistance;
        
        [Header("Chasing Stats")]
        [SerializeField] private float chasingSpeed;
        [SerializeField] private float chasingMaxDistance;
        
        [Header("Running Away Stats")]
        [SerializeField] private float runningAwaySpeed;
        [SerializeField] private float runningAwayMaxDistance;
        
        [Header("Jumping Stats")]
        [SerializeField] private float jumpForce;

        protected Vector3 RightLimit, LeftLimit;
        protected Vector3 ChasingRightLimit, ChasingLeftLimit;
        protected Vector3 RunningAwayLimit;
        protected bool ToRight = true;
        protected bool IsGrounded = false;
        protected MovementState State;
        protected PlayerSensor PlayerSensor;
        protected Rigidbody2D RigidBody;
        
        protected virtual void Awake()
        {
            PlayerSensor = GetComponentInChildren<PlayerSensor>();
            RigidBody = GetComponentInParent<Rigidbody2D>();
            
            RightLimit = transform.position;
            RightLimit.x += maxDistance;
            LeftLimit = transform.position;
            LeftLimit.x -= maxDistance;

            ChasingRightLimit = transform.position;
            ChasingRightLimit.x += chasingMaxDistance;
            ChasingLeftLimit = transform.position;
            ChasingLeftLimit.x -= chasingMaxDistance;
            
            State = MovementState.Patrolling;
        }

        public virtual void SetRunningAway()
        {
            State = MovementState.RunningAway;

            ToRight = !ToRight;
            Flip();

            var positionX = transform.position.x;
            var positionY = transform.position.y;
            var positionZ = transform.position.z;
            
            positionX += ToRight ? runningAwayMaxDistance : -runningAwayMaxDistance;
            RunningAwayLimit = new Vector3(positionX, positionY, positionZ);
        }

        protected virtual void FixedUpdate()
        {
            UpdateState();
            
            switch (State)
            {
                case MovementState.Patrolling:
                    Patrol();
                    break;
                case MovementState.Chasing:
                    Chasing();
                    break;
                case MovementState.RunningAway:
                    RunningAway();
                    break;
                default:
                    break;
            }
        }
        
        protected virtual void UpdateState()
        {
            if (RigidBody != null)
                RigidBody.velocity = new Vector2(0f, RigidBody.velocity.y);

            switch (State)
            {
                case MovementState.Patrolling:
                    PatrolCheckPlayerSensor();
                    break;
                case MovementState.Chasing:
                    ChasingCheckPlayerSensor();
                    break;
                case MovementState.RunningAway:
                    RunningAwayCheckDistance();
                    break;
                default:
                    break;
            }
        }

        protected virtual bool IsFacingPlayer(Transform playerTransform)
        {
            var directionToPlayer =  playerTransform.position.x - transform.position.x;
            return Mathf.Approximately(Mathf.Sign(directionToPlayer), Mathf.Sign(transform.localScale.x));
        }

        protected virtual void PatrolCheckPlayerSensor()
        {
            if (PlayerSensor.Collider == null) return;
            if (IsFacingPlayer(PlayerSensor.Collider.transform))
                State = MovementState.Chasing;
        }

        protected virtual void ChasingCheckPlayerSensor()
        {
            if (PlayerSensor.Collider == null) 
                State = MovementState.Patrolling;
        }

        protected virtual void Patrol()
        {
            UpdatePatrolDirection();
            MoveForward();
        }

        protected virtual void UpdatePatrolDirection()
        {
            ToRight = ToRight switch
            {
                true when transform.position.x >= RightLimit.x => false,
                false when transform.position.x <= LeftLimit.x => true,
                _ => ToRight
            };
            
            var scaleX = transform.localScale.x;
            
            if ((ToRight && scaleX < 0f) || (!ToRight && scaleX > 0f))
                Flip();
        }

        protected virtual void Chasing()
        {
            UpdateChasingDirection();
            if (CanMoveInChasing())
                MoveForward();
        }

        protected virtual bool CanMoveInChasing()
        {
            var canMove = !((ToRight && transform.position.x >= ChasingRightLimit.x) ||
                            (!ToRight && transform.position.x <= ChasingLeftLimit.x));

            var playerPosition = PlayerSensor.Collider.transform.position;
            var playerDistance = Mathf.Abs(playerPosition.x - transform.position.x);

            canMove = canMove && playerDistance > minDistanceToPlayer;
            return canMove;
        }

        protected virtual void UpdateChasingDirection()
        {
            if (IsFacingPlayer(PlayerSensor.Collider.transform)) return;
            
            ToRight = !ToRight;
            Flip();
        }

        protected virtual void RunningAway()
        {
            MoveForward();
        }

        protected virtual void RunningAwayCheckDistance()
        {
            var reachedLimit = ToRight ? transform.position.x >= RunningAwayLimit.x : transform.position.x <= RunningAwayLimit.x;

            if (!reachedLimit) return;
            
            ToRight = !ToRight;
            Flip();
                
            State = PlayerSensor.Collider != null ? MovementState.Chasing : MovementState.Patrolling;
        }

        protected virtual void Flip()
        {
            var scaleX = transform.localScale.x;
            var scaleY = transform.localScale.y;
            var scaleZ = transform.localScale.z;
            transform.localScale = new Vector3(-scaleX, scaleY, scaleZ);
        }

        protected virtual void MoveForward()
        {
            if (RigidBody == null) return;

            var currSpeed = State switch
            {
                MovementState.Chasing => chasingSpeed,
                MovementState.RunningAway => runningAwaySpeed,
                _ => this.speed
            };

            var direction = ToRight ? 1f : -1f;
            RigidBody.velocity = new Vector2(direction * currSpeed, RigidBody.velocity.y);
        }

        public virtual void Jump()
        {
            if (RigidBody != null && IsGrounded)
            {
                // Zera a velocidade Y antes do pulo para evitar pulos acumulados
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0f);
                RigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                IsGrounded = false;
            }
        }

        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            IsGrounded = false;

            foreach (ContactPoint2D contactPoint in collision.contacts)
            {
                if (contactPoint.normal.y > 0.5f)
                {
                    IsGrounded = true;
                    break;
                }
            }
        }

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            IsGrounded = false;
        }
    }
}