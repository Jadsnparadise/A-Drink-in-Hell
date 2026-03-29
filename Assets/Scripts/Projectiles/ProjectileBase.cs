using System;
using System.Collections;
using System.Collections.Generic;
using Effects;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Projectiles
{
    public class ProjectileBase : MonoBehaviour
    {
        private static readonly int Impact = Animator.StringToHash("Impact");

        public enum CollisionType
        {
            Player,
            Enemies,
            Walls,
        };
        
        [Header("Projectile Settings")]
        [SerializeField] protected float speed;
        [SerializeField] protected float timeToLive;
        [SerializeField] protected Vector2 direction;
        [SerializeField] protected EffectData effectData;
        [SerializeField] protected List<CollisionType> collisions = new(){CollisionType.Player};
        [SerializeField] protected bool bounce = false;
        [SerializeField] protected int maxBounces = 3;
        [SerializeField] protected Color colorModifier = Color.clear;

        protected HashSet<CollisionType> CollisionSet = new();
        protected Rigidbody2D RigidBody;
        protected Collider2D Collider;
        protected SpriteRenderer SpriteRenderer;
        protected Animator Animator;
        protected float TimeOfBirth;
        protected int BouncesCount;
        protected float ImpactTime;

        public bool CollidesWith(CollisionType type)
        {
            return CollisionSet.Contains(type);
        }

        protected void OnValidate()
        {
            if (maxBounces < 0)
                Debug.LogWarning("Max Bounces must be greater than 0");
        }

        protected virtual void Awake()
        {
            RigidBody = GetComponentInParent<Rigidbody2D>();
            Collider = GetComponentInParent<Collider2D>();
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            Animator = GetComponentInChildren<Animator>();
            
            BouncesCount = 0;
            ImpactTime = 0;
            CollisionSet.AddRange(collisions);
            
            if (!CollisionSet.Contains(CollisionType.Walls))
                Collider.isTrigger = true;
            if (colorModifier != Color.clear)
                SpriteRenderer.color = colorModifier;

        }

        protected virtual void Start()
        {
            TimeOfBirth = Time.time;
            RotateToDirection();
        }

        protected virtual void FixedUpdate()
        {
            Move();
            UpdateTimeToLive();
            CheckImpactTimeout();
        }

        protected virtual void UpdateTimeToLive()
        {
            if (Time.time - TimeOfBirth > timeToLive)
                ImpactAction();
        }

        protected virtual void CheckImpactTimeout()
        {
            if (ImpactTime != 0 && Time.time - ImpactTime > 0.5f)
                DestroyObject();
        }

        protected virtual void Move()
        {
            RigidBody.velocity = direction.normalized * speed;
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            var otherCollider = other.collider;
            
            OnTriggerEnter2D(otherCollider);
            if (IsWall(otherCollider) && CollisionSet.Contains(CollisionType.Walls))
                OnHitWall(other);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && CollisionSet.Contains(CollisionType.Player))
                OnHitPlayer(other);
            else if (other.CompareTag("Enemy") && CollisionSet.Contains(CollisionType.Enemies))
                OnHitEnemy(other);
        }

        protected virtual void OnHitPlayer(Collider2D player)
        {
            if  (effectData == null) return;
            var effect = effectData.CreateEffect();
            if (!player.TryGetComponent<PlayerEffectController>(out var controller)) return;
            controller.ApplyEffect(effect);
            
            ImpactAction();
            Collider.enabled = false;
        }

        protected virtual void OnHitEnemy(Collider2D enemy)
        {
            // Enemy not have EffectController
            ImpactAction();
        }

        protected virtual void OnHitWall(Collision2D wall)
        {
            if (bounce) Rebut(wall);
            else ImpactAction();
        }

        protected void Rebut(Collision2D wall)
        {
            if (BouncesCount >= maxBounces) 
                ImpactAction();
            
            var normal = wall.contacts[0].normal;
            direction = Vector2.Reflect(direction, normal).normalized;
            RigidBody.velocity = direction * speed;
            RotateToDirection();
            
            ++BouncesCount;
        }

        protected virtual void DestroyObject()
        {
            Destroy(gameObject);
        }
        
        protected virtual void ImpactAction()
        {
            if (ImpactTime != 0) return;
            
            if (Animator)  Animator.SetTrigger(Impact);
            else DestroyObject();
            ImpactTime = Time.time;
        }
        
        public virtual void Launch(Vector2 dir)
        {
            direction = dir.normalized;
        }
        
        protected virtual void RotateToDirection()
        {
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private static bool IsWall(Collider2D otherCollider)
        {
            return otherCollider.TryGetComponent<TilemapCollider2D>(out var col);
        }
    }
}