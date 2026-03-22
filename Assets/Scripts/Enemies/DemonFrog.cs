using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFrog : MonoBehaviour
{
    private Enemy _enemy;
    private EnemyMovement _movement;
    private EnemyAttack _attack;
    private Rigidbody2D _rb;
    private Animator _animator;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    
    private float _jumpTimer;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _movement = GetComponent<EnemyMovement>();
        _attack = GetComponent<EnemyAttack>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleJump();
        UpdateAnimations();
    }

    private void HandleJump()
    {
        _jumpTimer += Time.deltaTime;
        if (_jumpTimer >= jumpCooldown && Mathf.Abs(_rb.velocity.y) < 0.01)
        {
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _jumpTimer = 0;
        }
    }

    private void UpdateAnimations()
    {
        bool isMoving = Mathf.Abs(_rb.velocity.x) > 0.1f;
        _animator.SetBool("isMoving", isMoving);
        
        bool isJumping = Mathf.Abs(_rb.velocity.y) > 0.1f;
        _animator.SetBool("isJumping", isJumping);
    }
    
    public void TriggerAttackAnimation()
    {
        _animator.SetTrigger("attack");
    }
}
