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
                StopCoroutine(activeEffect.Coroutine);
                _activeEffects.Remove(type);
                StartCoroutine(ReplaceEffectRoutine(activeEffect.Effect, effect));
                return;
            }
            
            _activeEffects[type] = new ActiveEffect(effect, StartCoroutine(EffectRoutine(effect)));
        }

        private IEnumerator ReplaceEffectRoutine(PlayerEffect oldEffect, PlayerEffect newEffect)
        {
            var type = newEffect.GetType();
            
            yield return oldEffect.Remove(_playerController);
            
            _activeEffects[type] = new ActiveEffect(newEffect, StartCoroutine(EffectRoutine(newEffect)));
        }

        private IEnumerator EffectRoutine(PlayerEffect effect)
        {
            var type = effect.GetType();
            
            effect.Apply(_playerController);
            yield return new WaitForSeconds(effect.Duration);
            yield return effect.Remove(_playerController);
            
            _activeEffects.Remove(type);
        }
    }
}