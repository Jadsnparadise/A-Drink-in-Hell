using UnityEngine;

namespace Effects.Darkness
{
    [CreateAssetMenu(menuName = "Effects/Darkness Effect")]
    public class DarknessData : EffectData
    {
        [SerializeField] private float radius;
        public override PlayerEffect CreateEffect()
        {
            return new DarknessEffect(duration, radius);
        }
    }
}