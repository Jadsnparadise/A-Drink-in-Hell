using UnityEngine;
using DG.Tweening;

public class MoveableObstacle : Obstacle
{
    [SerializeField] private Transform endPoint;
    [SerializeField] private float duration = 2f;

    void Start()
    {
        transform.DOMove(endPoint.position, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = this.transform;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}