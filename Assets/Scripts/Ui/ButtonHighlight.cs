using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

namespace Games.UI
{
    [RequireComponent(typeof(Outline))]
    public class ButtonHighlight : MonoBehaviour
    {
        [SerializeField] private RectTransform highlightRect; // the moving highlight box
        [SerializeField] private Outline outline;
        [SerializeField] private Color idleColor = new Color(1f, 1f, 1f, 0f);
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private float moveDuration = 0.15f;
        [SerializeField] private float pulseInDuration = 0.15f;
        [SerializeField] private float pulseOutDuration = 0.3f;

        Color _currentColor;

        void Awake()
        {
            if (outline == null) outline = GetComponent<Outline>();
            _currentColor = outline.effectColor;
        }

        public void MoveTo(RectTransform target)
        {
            Tween.UIAnchoredPosition(highlightRect, target.anchoredPosition, moveDuration, Ease.OutCubic);
            Tween.Custom(this, highlightRect.sizeDelta, target.sizeDelta, moveDuration,
                (t, val) => t.highlightRect.sizeDelta = val, Ease.OutCubic);
            Pulse();
        }

        void Pulse()
        {
            Tween.Custom(this, _currentColor, highlightColor, pulseInDuration,
                    (t, c) => { t._currentColor = c; t.outline.effectColor = c; }, Ease.OutSine)
                .OnComplete(this, t => t.FadeOut());
        }

        void FadeOut()
        {
            Tween.Custom(this, highlightColor, idleColor, pulseOutDuration,
                (t, c) => { t._currentColor = c; t.outline.effectColor = c; });
        }
    }
}