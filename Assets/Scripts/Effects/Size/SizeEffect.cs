using System.Collections;
using UnityEngine;

namespace Effects.Size
{
    public class SizeEffect : PlayerEffect
    {
        private readonly float _sizeMultiplier;
        private readonly bool _cameraIncluded;
        private readonly bool _speedIncluded;
        
        public SizeEffect(float duration, float sizeMultiplier, bool cameraIncluded, bool speedIncluded) : base(duration)
        {
            _sizeMultiplier = sizeMultiplier;
            _cameraIncluded = cameraIncluded;
            _speedIncluded = speedIncluded;
        }

        public override void Apply(PlayerController controller)
        {
            var targetScale = controller.transform.localScale * _sizeMultiplier;
            controller.StartCoroutine(ChangeSize(targetScale, controller));
            
            if (_speedIncluded)
                controller.MultiplySpeed(_sizeMultiplier);

            var cam = Camera.main;
            if (_cameraIncluded && cam != null)
                controller.StartCoroutine(ChangeZoom(cam.orthographicSize * _sizeMultiplier));
        }

        public override void Remove(PlayerController controller)
        {
            var targetScale = controller.transform.localScale / _sizeMultiplier;
            controller.StartCoroutine(ChangeSize(targetScale, controller));
            
            if (_speedIncluded)
                controller.MultiplySpeed(1 / _sizeMultiplier);

            var cam = Camera.main;
            if (_cameraIncluded && cam != null)
                controller.StartCoroutine(ChangeZoom(cam.orthographicSize / _sizeMultiplier));
        }

        private IEnumerator ChangeSize(Vector3 targetScale, PlayerController controller)
        {
            var playerTransform = controller.transform;
            var velocity = Vector3.zero;
            
            while (Mathf.Abs((playerTransform.localScale - targetScale).sqrMagnitude) > 0.1f)
            {
                playerTransform.localScale = Vector3.SmoothDamp(
                    playerTransform.localScale,
                    targetScale,
                    ref velocity,
                    0.5f
                );
                yield return null;
            }

            playerTransform.localScale = targetScale;
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