using UnityEngine;

namespace Effects.Size
{
    [CreateAssetMenu(menuName = "Effects/Size Effect")]
    public class SizeData : EffectData
    {
        [SerializeField] private float sizeMultiplier;
        [SerializeField] private bool cameraIncluded = true;
        [SerializeField] private bool speedIncluded = true;
        [SerializeField] private bool jumpIncluded;
        
        public override PlayerEffect CreateEffect()
        {
            return new SizeEffect(duration, sizeMultiplier, cameraIncluded, speedIncluded, jumpIncluded);
        }
    }
}