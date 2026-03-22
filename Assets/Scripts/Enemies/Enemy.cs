using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float hitInvulnerability = 0.5f;
    
    Health _health;
    
    bool canTakeDamage = true;
    SpriteRenderer sprite;

    void Awake()
    {
        _health = new Health(maxHealth);
        sprite = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;

        _health.TakeDamage(damage);
        if (_health.IsDead())
        {
            Die();
            return;
        }

        StartCoroutine(DamageCooldown());
    }

    IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        for (int i = 0; i < 3; i++)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(hitInvulnerability);
        canTakeDamage = true;
    }
    
    void Die()
    {
        Destroy(gameObject);
    }
}
