using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Garante que o objeto tenha um AudioSource para o som
[RequireComponent(typeof(AudioSource))]
public class ButtonMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale")]
    [SerializeField] private bool useScale = true;
    [SerializeField] private Vector3 scaleFactor = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] private float scaleSpeed = 15f;

    [Header("Color")]
    [SerializeField] private bool useColor = true;
    [SerializeField] private Color hoverColor = Color.gray;
    [SerializeField] private float colorSpeed = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private float volume = 0.5f;

    // Referências e Variáveis de Controle
    private Image buttonImage;
    private AudioSource audioSource;

    private Vector3 initialScale;
    private Vector3 targetScale;

    private Color initialColor;
    private Color targetColor;

    void Awake()
    {
        // Pega as referências necessárias
        buttonImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();

        // Configuração básica do AudioSource para UI
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // Som 2D
    }

    void Start()
    {
        // Salva os estados iniciais
        if (useScale)
        {
            initialScale = transform.localScale;
            targetScale = initialScale;
        }

        if (useColor && buttonImage != null)
        {
            initialColor = buttonImage.color;
            targetColor = initialColor;
        }
        else if (useColor && buttonImage == null)
        {
            Debug.LogWarning($"O objeto {gameObject.name} não tem um componente 'Image' para mudar de cor.");
            useColor = false;
        }
    }

    void Update()
    {
        // Aplica a interpolação suave de Escala
        if (useScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }

        // Aplica a interpolação suave de Cor
        if (useColor && buttonImage != null)
        {
            buttonImage.color = Color.Lerp(buttonImage.color, targetColor, Time.deltaTime * colorSpeed);
        }
    }

    // --- Detectores do Mouse ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Define os alvos para "Mês em cima"
        if (useScale) targetScale = Vector3.Scale(initialScale, scaleFactor);
        if (useColor) targetColor = hoverColor;

        // Toca o som (uma vez)
        if (hoverSound != null && audioSource != null)
        {
            // PlayOneShot é ótimo porque não corta sons que já estão tocando
            audioSource.PlayOneShot(hoverSound, volume);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Define os alvos para "Mês saiu" (volta ao normal)
        if (useScale) targetScale = initialScale;
        if (useColor) targetColor = initialColor;
    }

    // Garante que o botão volte ao normal se for desativado
    void OnDisable()
    {
        if (useScale) transform.localScale = initialScale;
        if (useColor && buttonImage != null) buttonImage.color = initialColor;
    }
}
