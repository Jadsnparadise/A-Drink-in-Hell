using System;
using System.Collections;
using System.Collections.Generic;
using Effects;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerEffectController : MonoBehaviour
    {
        private class ActiveEffect
        {
            public readonly PlayerEffect Effect;
            public readonly Coroutine Coroutine;

            public ActiveEffect(PlayerEffect effect, Coroutine coroutine)
            {
                Effect = effect;
                Coroutine = coroutine;
            }
        }
        
        private PlayerController _playerController;
        private readonly Dictionary<Type, ActiveEffect> _activeEffects = new();
        
        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        public void ApplyEffect(PlayerEffect effect)
        {
            var type = effect.GetType();

            if (effect.IsStackable)
            {
                StartCoroutine(EffectRoutine(effect));
                return;
            }
            
            if (_activeEffects.TryGetValue(type, out var activeEffect))
            {
                activeEffect.Effect.Remove(_playerController);
                StopCoroutine(activeEffect.Coroutine);
            }
            
            _activeEffects[type] = new ActiveEffect(effect, StartCoroutine(EffectRoutine(effect)));
        }

        private IEnumerator EffectRoutine(PlayerEffect effect)
        {
            var type = effect.GetType();
            
            effect.Apply(_playerController);
            yield return new WaitForSeconds(effect.Duration);
            effect.Remove(_playerController);
            
            _activeEffects.Remove(type);
        }
    }
}