using System;
using Effects;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;


public class EffectSurface : MonoBehaviour
{
    private enum EffectType
    {
        Damage,
        Teleport,
        SpecialEffect
    }
    
    [Header("Effects")]
    [SerializeField] private EffectType effectType;
    
    [Header("Damage")]
    [SerializeField] private int damage = 0;
    [SerializeField] private float damageCooldown = 0;
    private float _lastTimeDamage = 0;
    
    [Header("Teleport")]
    [SerializeField] private Transform teleport = null;
    [SerializeField] private float teleportDistance = 0;
    
    [Header("Effect")]
    [SerializeField] private EffectData effectData = null;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private bool upwardKnockbackOnly = false;

    private Transform _player = null;
    private bool _playerInside = false;

    private void OnValidate()
    {
        if (!System.Enum.IsDefined(typeof(EffectType), effectType))
        {
            Debug.LogError($"{nameof(effectType)} is not defined");
        }

        if (effectType == EffectType.SpecialEffect && effectData == null)
        {
            Debug.LogError($"{nameof(effectData)} is not defined");
        }
    }

    private void OnCollisionEnter2D(Collision2D other) => OnEnter(other.collider);
    private void OnTriggerEnter2D(Collider2D other) => OnEnter(other);
    private void OnCollisionExit2D(Collision2D other) => OnExit(other.collider);
    private void OnTriggerExit2D(Collider2D other) => OnExit(other);

    private void OnEnter(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        _player = other.transform;
        _playerInside = true;
    }

    private void OnExit(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        _playerInside = false;
        _player = null;
    }

    private void Update()
    {
        if (!_playerInside) return;
        ApplyEffect();
    }

    private void ApplyEffect()
    {
        switch (effectType)
        {
            case EffectType.Damage:
                ApplyDamage();
                break;
            case EffectType.Teleport:
                ApplyTeleport();
                break;
            case EffectType.SpecialEffect:
                ApplySpecialEffect();
                break;
            default:
                break;
        }
    }

    private void ApplyDamage()
    {
        if (damageCooldown == 0 && _lastTimeDamage != 0) return;
        if (Time.time < _lastTimeDamage + damageCooldown) return;
        
        GameManager.Instance.DamagePlayer(damage);
        _lastTimeDamage = Time.time;
    }

    private void ApplyTeleport()
    {
        var target = teleport != null ? teleport : _player.transform;
        var displacement = teleportDistance > 0 ? Random.Range(-teleportDistance, teleportDistance) : 0;
        
        var position = target.position;
        position.x += displacement;
        
        _player.position = position;
    }

    private void ApplySpecialEffect()
    {
        if (effectData == null) return;
        var effect = effectData.CreateEffect();
        if (!_player.TryGetComponent<PlayerEffectController>(out var controller)) return;
        controller.ApplyEffect(effect);
    }
}
