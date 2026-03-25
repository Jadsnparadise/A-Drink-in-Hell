using UnityEngine;
using DG.Tweening;

public class PortalController : MonoBehaviour
{
    [SerializeField] private GameObject floorLeft;
    [SerializeField] private GameObject floorRight;

    [Header("Configurações")]
    [SerializeField] private float openDistance = 5f;
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private float offsetPlayer = 1f;
    [SerializeField] private Ease easeType = Ease.InOutQuad; 

    public void OpenPortal()
    {
        transform.position = new Vector2(PlayerController.Instance.transform.position.x + offsetPlayer, transform.position.y);

        floorLeft.transform.DOMoveX(floorLeft.transform.position.x - openDistance, duration)
            .SetEase(easeType);

        floorRight.transform.DOMoveX(floorRight.transform.position.x + openDistance, duration)
            .SetEase(easeType);
    }

    public void ClosePortal()
    {
        floorLeft.transform.DOMoveX(floorLeft.transform.position.x + openDistance, duration)
            .SetEase(easeType);

        floorRight.transform.DOMoveX(floorRight.transform.position.x - openDistance, duration)
            .SetEase(easeType);
    }
}