using UnityEngine;
using DG.Tweening;

public class PortalController : MonoBehaviour
{
    [SerializeField] private GameObject floorLeft;
    [SerializeField] private GameObject floorRight;

    [Header("Configuracoes")]
    [SerializeField] private float openDistance = 5f;
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private float offsetPlayer = 1f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;

    private float floorLeftInitialX;

    void Start()
    {
        floorLeftInitialX = floorLeft.transform.position.x;
    }

    public void OpenPortal()
    {
        
        CameraFollow.Instance.LockCamera();

        floorLeft.transform.DOMoveX(floorLeftInitialX - openDistance, duration)
            .SetEase(easeType);

    }

    public void ClosePortal()
    {
        floorLeft.transform.DOMoveX(floorLeftInitialX + openDistance, duration)
            .SetEase(easeType);

    }
}