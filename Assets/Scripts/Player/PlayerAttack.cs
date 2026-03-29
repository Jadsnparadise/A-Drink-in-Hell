using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private static readonly int Attack1 = Animator.StringToHash("Attack");

    [Header("Attack Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float cooldown = 0.5f;
    
    [Header("Hitbox Settings")]
    [SerializeField] private Collider2D hitboxCollider;

    private float _lastAttackTime;
    private bool _isAttacking = false;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();
    }
    
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            Attack();
    }

    private void Attack()
    {
        if (_isAttacking)
            return;
        if (Time.time < _lastAttackTime + cooldown)
            return;
        
        _isAttacking = true;
        _animator.SetTrigger(Attack1);
        _lastAttackTime = Time.time;

        PerformAttack();
    }

    private void PerformAttack()
    {
        var enemyCollider = GetEnemyCollider();
        if (!enemyCollider) return;
        var enemy = enemyCollider.GetComponent<EnemyController>();
        enemy.TakeDamage(damage);
    }

    public void EndAttack()
    {
        _isAttacking = false;
    }

    public bool IsAttacking()
    {
        return _isAttacking;
    }
    
    private Collider2D GetEnemyCollider()
    {
        List<Collider2D> colliders = new();
        var filter = new ContactFilter2D() { useTriggers = false };
        Physics2D.OverlapCollider(hitboxCollider, filter, colliders);

        return colliders.FirstOrDefault(col => col.CompareTag("Enemy"));
    }
}
