using System;
using Effects;
using Player;
using UnityEngine;

namespace Pickups
{
    public class PickupController : MonoBehaviour
    {
        [SerializeField] private EffectData effectData;

        private void OnValidate()
        {
            if (effectData == null)
                Debug.LogWarning($"{nameof(effectData)} is null!");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            ApplyEffect(other);
            Destroy(gameObject);
        }

        private void ApplyEffect(Collider2D other)
        {
            if  (effectData == null) return;
            var effect = effectData.CreateEffect();
            
            if (!other.TryGetComponent<PlayerEffectController>(out var controller)) return;
            controller.ApplyEffect(effect);
        }
    }
}