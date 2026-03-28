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

    private void OnCollisionStay2D(Collision2D other) => HandleContact(other.collider);
    private void OnTriggerStay2D(Collider2D other) => HandleContact(other);

    private void HandleContact(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            ApplyEffect(other.transform);
    }

    private void ApplyEffect(Transform player)
    {
        switch (effectType)
        {
            case EffectType.Damage:
                ApplyDamage();
                break;
            case EffectType.Teleport:
                ApplyTeleport(player);
                break;
            case EffectType.SpecialEffect:
                ApplySpecialEffect(player);
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

    private void ApplyTeleport(Transform player)
    {
        var target = teleport != null ? teleport : player.transform;
        var displacement = teleportDistance > 0 ? Random.Range(-teleportDistance, teleportDistance) : 0;
        
        var position = target.position;
        position.x += displacement;
        
        player.position = position;
    }

    private void ApplySpecialEffect(Transform player)
    {
        if (effectData == null) return;
        var effect = effectData.CreateEffect();
        if (!player.TryGetComponent<PlayerEffectController>(out var controller)) return;
        controller.ApplyEffect(effect);
    }
}
