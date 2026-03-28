using UnityEngine;

namespace Effects.Healing
{
    [CreateAssetMenu(menuName = "Effects/Healing Effect")]
    public class HealingData : EffectData
    {
        [SerializeField] private int healAmount;
        
        public override PlayerEffect CreateEffect()
        {
            return new HealingEffect(duration, healAmount);
        }
    }
}