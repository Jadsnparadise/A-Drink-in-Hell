using UnityEngine;
using Random = UnityEngine.Random;


public class EffectSurface : MonoBehaviour
{
    private enum EffectType
    {
        Damage,
        Teleport
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

    private void OnValidate()
    {
        if (!System.Enum.IsDefined(typeof(EffectType), effectType))
        {
            Debug.LogError($"{nameof(effectType)} is not defined");
        }
    }

    private void OnCollisionStay2D(Collision2D other)
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
}
