using System.Collections;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected int maxHealth;
        [SerializeField] protected float damageCooldown;

        protected Health Health;
        protected float LastDamageTime;
        protected SpriteRenderer SpriteRenderer;
        protected EnemyAttack Attacker;
        
        private bool _isDead = false;

        protected virtual void Awake()
        {
            Health = new Health(maxHealth);
            LastDamageTime = 0;
            SpriteRenderer = GetComponentInParent<SpriteRenderer>();
            Attacker = GetComponentInParent<EnemyAttack>();
        }

        public virtual void TakeDamage(int amount)
        {
            if (!CanDamage()) return;
            Health.TakeDamage(amount);

            LastDamageTime = Time.time;
            StartCoroutine(DamageAnimation());

            if (Health.IsDead()) Die();
        }

        protected virtual bool CanDamage()
        {
            return Time.time >= LastDamageTime + damageCooldown;
        }

        protected virtual void Die()
        {
            Attacker.hitboxCollider.enabled = false;
            if (!_isDead)
                StartCoroutine(DieRoutine());
            _isDead = true;
        }

        protected virtual IEnumerator DieRoutine()
        {
            yield return StartCoroutine(DieAnimation());
            Destroy(gameObject);
        }

        protected virtual IEnumerator DamageAnimation()
        {
            for (var i = 0; i < 3; i++)
            {
                SpriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                SpriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
        }

        protected virtual IEnumerator DieAnimation()
        {
            var targetScale = Vector3.one * 0.2f;
            var velocity = Vector3.zero;

            while ((transform.localScale - targetScale).sqrMagnitude > 0.01f)
            {
                transform.localScale = Vector3.SmoothDamp(
                    transform.localScale,
                    targetScale,
                    ref velocity,
                    0.3f
                );
                yield return null;
            }
            transform.localScale = Vector3.zero;
        }
    }
}