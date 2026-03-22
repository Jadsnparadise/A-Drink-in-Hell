using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Collider2D attackCollider;

    private float _lastAttackTime;
    private bool _isAttacking = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            Attack();
    }

    void Attack()
    {
        if (_isAttacking)
            return;
        if (Time.time < _lastAttackTime + attackCooldown)
            return;
        
        _isAttacking = true;
        animator.SetTrigger("Attacking");
        _lastAttackTime = Time.time;
    }

    public void EndAttack()
    {
        _isAttacking = false;
    }

    public bool IsAttacking()
    {
        return _isAttacking;
    }

    private void EnableHitbox()
    {
        attackCollider.enabled = true;
    }

    private void DisableHitbox()
    {
        attackCollider.enabled = false;
    }
}
