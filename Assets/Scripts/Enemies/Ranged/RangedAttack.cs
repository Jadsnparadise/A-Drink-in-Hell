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

        private bool _isBursting = false;

        protected override void Update()
        {
            var player = GetPlayerCollider();
            if (!player || _isBursting || !CanAttack()) return;

            StartCoroutine(BurstAttackSequence(player));
        }

        private IEnumerator BurstAttackSequence(Collider2D player)
        {
            _isBursting = true;

            for (var i = 0; i < burstCount; i++)
            {
                var currentPlayer = GetPlayerCollider();
                if (currentPlayer)
                {
                    if (Animator) Animator.SetTrigger(Attack);
                    PerformAttack(currentPlayer);
                }

                if (i < burstCount - 1)
                    yield return new WaitForSeconds(burstCooldown);
            }

            LastAttackTime = Time.time;
            _isBursting = false;
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

        protected virtual Vector2 GetAttackVector(Collider2D player)
        {
            var vectorToPlayer = (Vector2)(player.transform.position - transform.position);

            var playerRb = player.attachedRigidbody;
            if (playerRb)
                vectorToPlayer += playerRb.velocity * velocityPredictionFactor;

            return vectorToPlayer;
        }

        protected virtual void PerformAttack(Collider2D player)
        {
            var vectorToPlayer = GetAttackVector(player);
            var distance = vectorToPlayer.magnitude;
            var direction = vectorToPlayer.normalized;
            var spawnPosition = transform.position + (Vector3)(direction * projectileSpawnOffset);

            if (IsLineOfSightBlocked(direction, distance))
                return;

            var projInst = Instantiate(projectile, spawnPosition, Quaternion.identity);
            projInst.Launch(direction);
        }

        private bool IsLineOfSightBlocked(Vector2 direction, float distance)
        {
            if (!projectile.CollidesWith(ProjectileBase.CollisionType.Walls))
                return false;
            
            var slightlyElevatedOrigin = new Vector2(transform.position.x, transform.position.y + 0.5f);

            var hits = Physics2D.RaycastAll(slightlyElevatedOrigin, direction, distance);
            return hits.Any(hit => hit.collider.TryGetComponent<TilemapCollider2D>(out _));
        }
    }
}