using UnityEngine;
using UnityEngine.UI;   
using PrimeTween;

public class AnimaMenuButton : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    Color originalColor;

    [SerializeField] RectTransform buttonRect;
    [SerializeField] CanvasGroup buttonCanvasGroup;

    Vector2 originalAnchoredPos;
    Vector3 originalScale;

    void Awake()
    {
        originalAnchoredPos = buttonRect.anchoredPosition;
        originalScale = buttonRect.localScale;
        originalColor = buttonImage.color;
    }

    public void Playanimation()
    {
        buttonRect.gameObject.SetActive(true);

        Vector2 startOffset = new Vector2(-150f, -400f);
        Vector2 startPos = originalAnchoredPos + startOffset;

        buttonRect.anchoredPosition = startPos;
        buttonRect.localScale = new Vector3(0.05f, 1.4f, 1f);
        buttonRect.localRotation = Quaternion.Euler(80f, 0f, -25f);

        Sequence.Create()
            .Group(Tween.UIAnchoredPosition(
                buttonRect,
                endValue: originalAnchoredPos,
                duration: 0.28f,
                ease: Ease.OutExpo))
            .Group(Tween.LocalEulerAngles(
                buttonRect,
                startValue: new Vector3(80f, 0f, -25f),
                endValue: new Vector3(0f, 0f, 900f),
                duration: 0.4f,
                ease: Ease.OutCubic))
            .Group(Tween.Scale(
                buttonRect,
                endValue: originalScale * 1.3f,
                duration: 0.3f,
                ease: Ease.OutBack))
            .Chain(Tween.ShakeLocalPosition(
                buttonRect,
                strength: new Vector3(25f, 15f, 0f),
                duration: 0.18f,
                frequency: 30))
            .Insert(0.3f, Tween.Color(buttonImage, Color.white, duration: 0.08f))
            .Chain(Tween.Color(buttonImage, originalColor, duration: 0.15f))
            .Chain(Tween.Scale(
                buttonRect,
                endValue: originalScale,
                duration: 0.15f,
                ease: Ease.OutElastic))
            .ChainCallback(() => Debug.Log("Glitch-Warp animation complete"));
    }
}