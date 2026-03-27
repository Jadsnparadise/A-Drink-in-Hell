using UnityEngine;
using DG.Tweening;

public class MoveableObstacle : Obstacle
{
    [SerializeField] private Transform endPoint;
    [SerializeField] private float duration = 2f;

    public Vector2 PlatformVelocity;

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
        transform.DOMove(endPoint.position, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void FixedUpdate()
    {
        PlatformVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.currentPlatform = this;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.currentPlatform = null;
        }
    }
}