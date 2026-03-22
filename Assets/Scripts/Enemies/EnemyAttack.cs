using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private Collider2D attackZone;

    private EnemySensor _sensor;
    private float _lastAttack;

    private void Awake()
    {
        _sensor = GetComponentInChildren<EnemySensor>();
        attackZone.enabled = false;
    }

    void Update()
    {
        if (!_sensor.PlayerDetected || !PlayerInFront()) return;
        if (Time.time > _lastAttack + attackCooldown)
            Attack();
    }

    private void Attack()
    {
        _lastAttack = Time.time;
        
        GetComponent<DemonFrog>().TriggerAttackAnimation();
        
        attackZone.enabled = true;
        Invoke(nameof(DisableAttack), 0.2f);
    }

    private void DisableAttack()
    {
        attackZone.enabled = false;
    }

    private bool PlayerInFront()
    {
        float direction = transform.localScale.x;
        float playerDir = _sensor.player.position.x - transform.position.x;

        return Mathf.Sign(playerDir) == Mathf.Sign(direction);
    }
}
