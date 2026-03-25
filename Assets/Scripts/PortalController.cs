using UnityEngine;
using DG.Tweening; // Não esqueça do namespace!

public class PortalController : MonoBehaviour
{
    [SerializeField] private GameObject floorLeft;
    [SerializeField] private GameObject floorRight;

    [Header("Configurações")]
    [SerializeField] private float openDistance = 5f; // O quanto eles se afastam
    [SerializeField] private float duration = 1.2f;    // Tempo da animação
    [SerializeField] private float offsetPlayer = 1f;   // Distância do portal para o jogador
    [SerializeField] private Ease easeType = Ease.InOutQuad; // Tipo de suavização

    /// <summary>
    /// Abre o chão movendo as partes para os lados opostos.
    /// </summary>
    public void OpenPortal()
    {
        transform.position = new Vector2(PlayerController.Instance.transform.position.x + offsetPlayer, transform.position.y);
        // Move para a esquerda (valor negativo no X relativo à posição atual)
        floorLeft.transform.DOMoveX(floorLeft.transform.position.x - openDistance, duration)
            .SetEase(easeType);

        // Move para a direita (valor positivo no X relativo à posição atual)
        floorRight.transform.DOMoveX(floorRight.transform.position.x + openDistance, duration)
            .SetEase(easeType);
    }

    /// <summary>
    /// Fecha o chão retornando à posição original (opcional).
    /// </summary>
    public void ClosePortal()
    {
        // Se quiser que eles voltem exatamente para onde estavam antes
        floorLeft.transform.DOMoveX(floorLeft.transform.position.x + openDistance, duration)
            .SetEase(easeType);

        floorRight.transform.DOMoveX(floorRight.transform.position.x - openDistance, duration)
            .SetEase(easeType);
    }
}