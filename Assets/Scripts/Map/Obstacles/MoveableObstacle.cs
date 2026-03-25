using UnityEngine;
using DG.Tweening;

public class MoveableObstacle : MonoBehaviour
{
    [SerializeField] private Transform endPoint;
    [SerializeField] private float duration = 2f;

    void Start()
    {
        transform.DOMove(endPoint.position, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}