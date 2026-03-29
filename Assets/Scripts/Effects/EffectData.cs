using UnityEngine;

namespace Effects
{
    public abstract class EffectData : ScriptableObject
    {
        [SerializeField] protected float duration;
        
        public abstract PlayerEffect CreateEffect();
    }
}