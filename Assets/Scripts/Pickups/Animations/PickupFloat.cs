using UnityEngine;

namespace Pickups.Animations
{
    public class PickupFloat : MonoBehaviour
    {
        [SerializeField] private float amplitude = 0.01f;
        [SerializeField] private float frequency = 3f;

        private Vector3 _startPos;

        private void Start()
        {
            _startPos = transform.localPosition;
        }

        private void Update()
        {
            var y = Mathf.Sin(Time.time * frequency) * amplitude;
            transform.localPosition = _startPos + new Vector3(0, y, 0);
        }
    }
}