using Effects;
using UnityEngine;

namespace Pickups.Animations
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PickupGlow : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float intensity = 0.1f;
        
        private SpriteRenderer _renderer;
        private float _defaultIntensity;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _defaultIntensity = 1 - 2 * intensity;
        }

        private void Update()
        {
            var alpha = _defaultIntensity + Mathf.Sin(Time.time * speed) * intensity;
            var color = _renderer.color;
            color.a = alpha;
            _renderer.color = color;
        }
    }
}