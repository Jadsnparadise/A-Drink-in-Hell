using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class ButtonMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale")]
    [SerializeField] private bool useScale = true;
    [SerializeField] private Vector3 scaleFactor = new(1.1f, 1.1f, 1.1f);
    [SerializeField] private float scaleSpeed = 15f;

    [Header("Color")]
    [SerializeField] private bool useColor = true;
    [SerializeField] private Color hoverColor = Color.gray;
    [SerializeField] private float colorSpeed = 10f;

    [Header("Text")]
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Color textHoverColor = Color.yellow;
    [SerializeField] private Color textDefaultColor = Color.white;

    [Header("Audio")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private float volume = 0.5f;

    private Image buttonImage;
    private AudioSource audioSource;

    private Vector3 initialScale;
    private Vector3 targetScale;

    private Color initialColor;
    private Color targetColor;

    private bool isHovered;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();

        SetupInitialValues();
        SetupAudio();
    }

    private void SetupInitialValues()
    {
        // SCALE
        initialScale = transform.localScale == Vector3.zero ? Vector3.one : transform.localScale;
        targetScale = initialScale;

        // COLOR
        if (buttonImage != null)
        {
            initialColor = buttonImage.color;
            targetColor = initialColor;
        }
        else if (useColor)
        {
            Debug.LogWarning($"{name} não possui Image. Desativando uso de cor.");
            useColor = false;
        }
    }

    private void SetupAudio()
    {
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    private void Update()
    {
        UpdateScale();
        UpdateColor();
    }

    private void UpdateScale()
    {
        if (!useScale) return;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
        );
    }

    private void UpdateColor()
    {
        if (!useColor || buttonImage == null) return;

        buttonImage.color = Color.Lerp(
            buttonImage.color,
            targetColor,
            Time.deltaTime * colorSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        ApplyHoverState();
        PlayHoverSound();
        UpdateTextColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        ApplyDefaultState();
        UpdateTextColor();
    }

    private void ApplyHoverState()
    {
        if (useScale)
            targetScale = Vector3.Scale(initialScale, scaleFactor);

        if (useColor)
            targetColor = hoverColor;
    }

    private void ApplyDefaultState()
    {
        if (useScale)
            targetScale = initialScale;

        if (useColor)
            targetColor = initialColor;
    }

    private void UpdateTextColor()
    {
        if (buttonText == null) return;

        buttonText.color = isHovered ? textHoverColor : textDefaultColor;
    }

    private void PlayHoverSound()
    {
        if (hoverSound == null) return;

        audioSource.PlayOneShot(hoverSound, volume);
    }

    private void OnDisable()
    {
        ResetVisuals();
    }

    private void ResetVisuals()
    {
        if (useScale)
            transform.localScale = initialScale;

        if (useColor && buttonImage != null)
            buttonImage.color = initialColor;

        if (buttonText != null)
            buttonText.color = textDefaultColor;

        isHovered = false;
    }
}