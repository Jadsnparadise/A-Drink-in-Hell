using UnityEngine;

namespace Effects.CameraCrop
{
    [CreateAssetMenu(menuName = "Effects/Camera Crop Effect")]
    public class CameraCropData : EffectData
    {
        [SerializeField] private float cropMultiplier;

        public override PlayerEffect CreateEffect()
        {
            return new CameraCropEffect(duration, cropMultiplier);
        }
    }
}