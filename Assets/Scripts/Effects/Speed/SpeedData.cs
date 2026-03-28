using UnityEngine;

namespace Effects.Speed
{
    [CreateAssetMenu(menuName = "Effects/Speed Effect")]
    public class SpeedData : EffectData
    {
        [SerializeField] private float speedMultiplier = 1.5f;
        
        public override PlayerEffect CreateEffect()
        {
            return new SpeedEffect(duration, speedMultiplier);
        }
    }
}