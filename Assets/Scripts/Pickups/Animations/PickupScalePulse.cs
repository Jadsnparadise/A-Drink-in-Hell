using UnityEngine;

namespace Pickups.Animations
{
    public class PickupScalePulse : MonoBehaviour
    {
        [SerializeField] private float amount = 0.1f;
        [SerializeField] private float speed = 5f;

        private Vector3 _startScale;

        private void Start()
        {
            _startScale = transform.localScale;
        }

        private void Update()
        {
            var scale = 1 + Mathf.Sin(Time.time * speed) * amount;
            transform.localScale = _startScale * scale;
        }
    }
}