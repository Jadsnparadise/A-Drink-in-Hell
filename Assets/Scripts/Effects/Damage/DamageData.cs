using UnityEngine;

namespace Effects.Damage
{
    [CreateAssetMenu(menuName = "Effects/Damage Effect")]
    public class DamageData : EffectData
    {
        [SerializeField] private int damage;
        
        public override PlayerEffect CreateEffect()
        {
            return new DamageEffect(duration, damage);
        }
    }
}