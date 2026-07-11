using UnityEngine;
using UnityEngine.EventSystems;
using PrimeTween;



public class MenuButtonAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rectTransform;
 
    [Header("Expand")]
    [SerializeField] private float expandedScale = 1.15f;
    [SerializeField] private float expandDuration = 0.2f;
 
    [Header("Shift Right")]
    [SerializeField] private float shiftDistance = 40f;
    [SerializeField] private float shiftDuration = 0.25f;
 
    [Header("Rotate")]
    [Tooltip("Degrees to rotate on the Z axis (flat 2D rotation).")]
    [SerializeField] private float rotateAngle = 25f;
    [SerializeField] private float rotateDuration = 0.25f;
 
    [SerializeField] private Ease ease = Ease.OutBack;
 
    private Vector3 _initialScale;
    private Vector2 _initialAnchoredPosition;
    private Quaternion _initialRotation;

    private void Awake()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        _initialScale = rectTransform.localScale;
        _initialAnchoredPosition = rectTransform.anchoredPosition;
        _initialRotation = rectTransform.localRotation;
    }
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        Sequence.Create()
            // 1. Expand first.
            .Chain(Tween.Scale(rectTransform, endValue: _initialScale * expandedScale, duration: expandDuration, ease: ease))
            // 2. Then shift right and rotate together (Group runs in parallel with the Chain right before it).
            .Chain(Tween.UIAnchoredPositionX(rectTransform, endValue: _initialAnchoredPosition.x + shiftDistance, duration: shiftDuration, ease: ease))
            .Group(Tween.Rotation(rectTransform, endValue: Quaternion.Euler(0, 0, -rotateAngle), duration: rotateDuration, ease: ease));
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        Sequence.Create()
            // Reverse: shift back + un-rotate together, then shrink back down.
            .Chain(Tween.UIAnchoredPositionX(rectTransform, endValue: _initialAnchoredPosition.x, duration: shiftDuration, ease: ease))
            .Group(Tween.Rotation(rectTransform, endValue: _initialRotation, duration: rotateDuration, ease: ease))
            .Chain(Tween.Scale(rectTransform, endValue: _initialScale, duration: expandDuration, ease: ease));
    }
}
    

