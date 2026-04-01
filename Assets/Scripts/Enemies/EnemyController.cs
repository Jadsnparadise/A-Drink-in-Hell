using System.Collections;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyController : MonoBehaviour
    {
        private static readonly int Death = Animator.StringToHash("Death");

        [Header("Stats")]
        [SerializeField] protected int maxHealth;
        [SerializeField] protected float damageCooldown;

        protected Health Health;
        protected float LastDamageTime;
        protected SpriteRenderer SpriteRenderer;
        protected EnemyAttack Attacker;
        protected Rigidbody2D Rigidbody;
        protected Animator Animator;

        protected bool IsDeath = false;

        protected virtual void Awake()
        {
            Health = new Health(maxHealth);
            LastDamageTime = 0;
            SpriteRenderer = GetComponentInParent<SpriteRenderer>();
            Attacker = GetComponentInParent<EnemyAttack>();
            Animator = GetComponentInParent<Animator>();
            Rigidbody = GetComponentInParent<Rigidbody2D>();

            IsDeath = false;
        }

        public event System.Action<int> OnDamaged;

        public virtual void TakeDamage(int amount)
        {
            if (!CanDamage()) return;
            Health.TakeDamage(amount);

            LastDamageTime = Time.time;
            OnDamaged?.Invoke(amount);

            if (Health.IsDead()) Die();
            StartCoroutine(DamageAnimation());
        }

        protected virtual bool CanDamage()
        {
            return Time.time >= LastDamageTime + damageCooldown;
        }

        protected virtual void Die()
        {
            if (Attacker && Attacker.hitboxCollider)
                Attacker.hitboxCollider.enabled = false;

            if (Rigidbody)
                Rigidbody.velocity = new Vector2(0f, Rigidbody.velocity.y);
            
            if (!IsDeath && Animator)
                Animator.SetTrigger(Death);
            IsDeath = true;
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

        protected virtual void OnDeathAnimationEnd()
        {
            Destroy(gameObject);
        }

        public virtual bool IsDead()
        {
            return Health.IsDead();
        }
    }
}