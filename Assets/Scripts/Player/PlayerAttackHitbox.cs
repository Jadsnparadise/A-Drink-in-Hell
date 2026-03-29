using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        var enemy = other.GetComponent<EnemyController>(); 
        enemy.TakeDamage(damage);
    }
}
