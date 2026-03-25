using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;    
    [SerializeField] private Vector3 velocity = Vector3.zero;

    private bool isToFollow = true;

    void Update()
    {
        if(!isToFollow) return;

        transform.position = Vector3.SmoothDamp(
            transform.position, 
            PlayerController.Instance.transform.position + offset, 
            ref velocity,
            followSpeed
        );
    }

    public void SetFollowState(bool newState)
    {
        isToFollow = newState;
    }
}