using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class IngredientUIItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(Sprite icon)
    {
        if (icon != null && iconImage != null)
            iconImage.sprite = icon;

        amountText.text = "0/1";

        DOTween.Kill(rectTransform);
        DOTween.Kill(canvasGroup);
        canvasGroup.alpha = 1f;
    }

    public void MarkAsCollected()
    {
        amountText.text = "1/1";

        Sequence animSequence = DOTween.Sequence();
        animSequence.AppendInterval(1f);

        animSequence.Append(rectTransform.DOAnchorPos(rectTransform.anchoredPosition + new Vector2(150f, 0f), 0.5f));
        animSequence.Join(canvasGroup.DOFade(0f, 0.5f));
        animSequence.OnComplete(() => gameObject.SetActive(false));
    }
}