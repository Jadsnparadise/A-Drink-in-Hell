using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private InputSystemActions _input;

    private void Start()
    {
        ConfigureInput();
    }

    private void ConfigureInput()
    {
        _input = new InputSystemActions();
        _input.Player.Attack.Enable();
        _input.Player.Attack.performed += OnAttack;
    }
    

    public void OnDisable()
    {
        _input.Player.Attack.Disable();
    }

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
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
    }

    private void PerformAttack()
    {
        var enemyCollider = GetEnemyCollider();
        if (!enemyCollider) return;
        var enemy = enemyCollider.GetComponent<EnemyController>();
        enemy.TakeDamage(damage);
    }

    public bool IsAttacking()
    {
        if (_isAttacking && Time.time >= _lastAttackTime + cooldown)
            _isAttacking = false;
        return _isAttacking;
    }

    public void EndAttack()
    {
        _isAttacking = false;
    }
    
    private Collider2D GetEnemyCollider()
    {
        List<Collider2D> colliders = new();
        var filter = new ContactFilter2D() { useTriggers = false };
        Physics2D.OverlapCollider(hitboxCollider, filter, colliders);

        return colliders.FirstOrDefault(col => col.CompareTag("Enemy"));
    }
}
