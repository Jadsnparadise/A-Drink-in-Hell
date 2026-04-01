using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Projectiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Enemies.Ranged
{
    public class RangedAttack : EnemyAttack
    {
        [Header("Sensor")]
        [SerializeField] protected Collider2D sensor;

        [Header("Projectile")]
        [SerializeField] protected ProjectileBase projectile;
        [SerializeField][Range(0f, 2f)] protected float velocityPredictionFactor = 0.5f;
        [SerializeField] protected float projectileSpawnOffset = 0.2f;

        [Header("Burst Attack Settings")]
        [Tooltip("Number of attacks in sequence before big cooldown")]
        [SerializeField] protected int burstCount = 1;
        [Tooltip("Time between attacks in the sequence")]
        [SerializeField] protected float burstCooldown = 1f;

        protected int CurrentBurstCount = 0;

        protected override void Update()
        {
            if (Controller.IsDead()) return;

            var player = GetPlayerCollider();
            if (!player || !CanAttack() || !Animator) return;

            LastInitialAttackTime = Time.time;
            Attacking = true;
            Animator.SetTrigger(Attack);
        }

        protected override bool CanAttack()
        {
            float currentCooldown = CurrentBurstCount == 0 ? attackCooldown : burstCooldown;

            if (LastInitialAttackTime + currentCooldown <= Time.time)
                Attacking = false;

            return LastAttackTime + currentCooldown <= Time.time && !Attacking;
        }

        protected override Collider2D GetPlayerCollider()
        {
            var colliders = new List<Collider2D>();
            var filter = new ContactFilter2D { useTriggers = false };
            Physics2D.OverlapCollider(sensor, filter, colliders);

            var player = colliders.FirstOrDefault(col => col.CompareTag("Player"));

            if (player && IsFacingPlayer(player.transform))
            {
                return player;
            }

            return null;
        }

        private bool IsFacingPlayer(Transform playerTransform)
        {
            var directionToPlayer = playerTransform.position.x - transform.position.x;
            var isFacingRight = transform.localScale.x > 0;
            var isPlayerToRight = directionToPlayer > 0;

            return isFacingRight == isPlayerToRight;
        }

        protected virtual Vector3 GetOriginPoint()
        {
            var mainCollider = GetComponentInParent<Collider2D>();
            if (mainCollider) return mainCollider.bounds.center;

            if (hitboxCollider) return hitboxCollider.bounds.center;

            return transform.position + Vector3.up * 0.5f;
        }

        protected virtual Vector2 GetAttackVector(Collider2D player)
        {
            var myCenter = GetOriginPoint();
            var playerCenter = player.bounds.center;

            var vectorToPlayer = (Vector2)(playerCenter - myCenter);

            var playerRb = player.attachedRigidbody;
            if (playerRb)
                vectorToPlayer += playerRb.velocity * velocityPredictionFactor;

            return vectorToPlayer;
        }

        protected override void PerformAttack()
        {
            if (Controller.IsDead()) return;

            LastAttackTime = Time.time;
            CurrentBurstCount++;

            if (CurrentBurstCount >= burstCount)
                CurrentBurstCount = 0;


            var player = GetPlayerCollider();
            if (!player || Controller.IsDead()) return;

            var myCenter = GetOriginPoint();
            var vectorToPlayer = GetAttackVector(player);
            var distance = vectorToPlayer.magnitude;
            var direction = vectorToPlayer.normalized;

            var spawnPosition = myCenter + (Vector3)(direction * projectileSpawnOffset);

            if (IsLineOfSightBlocked(myCenter, direction, distance))
                return;

            var projInst = Instantiate(projectile, spawnPosition, Quaternion.identity);
            projInst.Launch(direction);

            if (runningAwayAfterAttacking && CurrentBurstCount == 0)
                StartCoroutine(ActiveRunningAway());
        }

        private bool IsLineOfSightBlocked(Vector2 origin, Vector2 direction, float distance)
        {
            if (!projectile.CollidesWith(ProjectileBase.CollisionType.Walls))
                return false;

            var hits = Physics2D.RaycastAll(origin, direction, distance);
            return hits.Any(hit => hit.collider.TryGetComponent<TilemapCollider2D>(out _));
        }
    }
}