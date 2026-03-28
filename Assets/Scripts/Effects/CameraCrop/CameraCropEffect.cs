using System.Collections;
using UnityEngine;

namespace Effects.CameraCrop
{
    public class CameraCropEffect : PlayerEffect
    {
        private readonly float _cropMultiplier;
        
        public CameraCropEffect(float duration, float cropMultiplier) : base(duration)
        {
            _cropMultiplier = cropMultiplier;
        }

        public override void Apply(PlayerController controller)
        {
            var cam = Camera.main;
            if (cam != null)
                controller.StartCoroutine(ChangeZoom(cam.orthographicSize / _cropMultiplier));
        }

        public override void Remove(PlayerController controller)
        {
            var cam = Camera.main;
            if (cam != null)
                controller.StartCoroutine(ChangeZoom(cam.orthographicSize * _cropMultiplier));
        }

        private IEnumerator ChangeZoom(float targetSize)
        {
            var cam = Camera.main!;
            var velocity = 0f;
            
            while (Mathf.Abs(cam.orthographicSize - targetSize) > 0.01f)
            {
                cam.orthographicSize = Mathf.SmoothDamp(
                    cam.orthographicSize,
                    targetSize,
                    ref velocity,
                    0.5f
                );
                yield return null;
            }
            
            cam.orthographicSize = targetSize;
        }
    }
}