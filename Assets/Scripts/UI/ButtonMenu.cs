using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [Header("Text")]
    [SerializeField] private TMP_Text buttonText;

    private Image buttonImage;
    private AudioSource audioSource;

    private Vector3 initialScale;
    private Vector3 targetScale;

    private Color initialColor;
    private Color targetColor;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();


        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    void Start()
    {
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
        if (useScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }

        if (useColor && buttonImage != null)
        {
            buttonImage.color = Color.Lerp(buttonImage.color, targetColor, Time.deltaTime * colorSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useScale) targetScale = Vector3.Scale(initialScale, scaleFactor);
        if (useColor) targetColor = hoverColor;

        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound, volume);
        }

        buttonText.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (useScale) targetScale = initialScale;
        if (useColor) targetColor = initialColor;

        buttonText.color = Color.white;
    }

    void OnDisable()
    {
        if (useScale) transform.localScale = initialScale;
        if (useColor && buttonImage != null) buttonImage.color = initialColor;
    }
}
