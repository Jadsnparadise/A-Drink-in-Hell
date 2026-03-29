using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Effects.Darkness
{
    public class DarknessEffect : PlayerEffect
    {
        private readonly float _radius;
        private Light2D _globalLight = null;
        private Light2D _playerLight = null;

        public DarknessEffect(float duration, float radius) : base(duration)
        {
            _radius = radius;
        }

        public override void Apply(PlayerController controller)
        {
            GetGlobalLight();
            if (_globalLight == null) return;
            controller.StartCoroutine(ChangeLightIntensity(0f, _globalLight));
            CreatePlayerLight(controller);
        }

        public override void Remove(PlayerController controller)
        {
            if (_globalLight == null) return;
            controller.StartCoroutine(ChangeLightIntensity(1f, _globalLight));

            if (_playerLight == null) return;
            controller.StartCoroutine(RemovePlayerLight(controller));
        }

        private void GetGlobalLight()
        {
            var lights = Object.FindObjectsByType<Light2D>(FindObjectsSortMode.None);
            foreach (var light in lights)
            {
                if (light.lightType != Light2D.LightType.Global) continue;
                _globalLight = light;
                break;
            }
        }

        private void CreatePlayerLight(PlayerController controller)
        {
            var lightObj = new GameObject("PlayerDarknessLight");
            lightObj.transform.SetParent(controller.transform);
            lightObj.transform.localPosition = Vector3.zero;

            _playerLight = lightObj.AddComponent<Light2D>();
            _playerLight.lightType = Light2D.LightType.Point;
            _playerLight.pointLightOuterRadius = _radius;
            _playerLight.pointLightInnerRadius = _radius * 0.2f;

            controller.StartCoroutine(ChangeLightIntensity(1f, _playerLight));
            _playerLight.color = Color.white;
        }

        private IEnumerator ChangeLightIntensity(float targetIntensity, Light2D light)
        {
            var velocity = 0f;
            while (Mathf.Abs(light.intensity - targetIntensity) > 0.01f)
            {
                light.intensity = Mathf.SmoothDamp(
                    light.intensity,
                    targetIntensity,
                    ref velocity,
                    0.5f
                );
                yield return null;
            }

            light.intensity = targetIntensity;
        }

        private IEnumerator RemovePlayerLight(PlayerController controller)
        {
            controller.StartCoroutine(ChangeLightIntensity(0f, _playerLight));
            yield return new WaitForSeconds(0.5f);
            Object.Destroy(_playerLight.gameObject);
        }
    }
}