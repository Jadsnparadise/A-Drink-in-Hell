using System.Collections;
using UnityEngine;

namespace Effects.Speed
{
    public class SpeedEffect : PlayerEffect
    {
        private readonly float _speedMultiplier;
        
        public SpeedEffect(float duration, float speedMultiplier) : base(duration)
        {
            _speedMultiplier = speedMultiplier;
        }


        public override void Apply(PlayerController controller)
        {
            controller.MultiplySpeed(_speedMultiplier);
        }

        public override IEnumerator Remove(PlayerController controller)
        {
            controller.MultiplySpeed(1 / _speedMultiplier);
            yield break;
        }
    }
}