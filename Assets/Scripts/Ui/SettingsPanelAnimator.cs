using System;
using UnityEngine;
using PrimeTween;

namespace Games.UI
{
    public class SettingsPanelAnimator : MonoBehaviour
    {
        [Header("Panel Parts")]
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private RectTransform outerFrame;
        [SerializeField] private RectTransform innerContent;
        [SerializeField] private RectTransform[] buttons;

        [Header("Motion")]
        [SerializeField] private Vector2 offscreenOffset = new Vector2(800f, 0f);
        [SerializeField] private float moveDuration = 0.18f;
        [SerializeField] private float doorSwingDuration = 0.3f;
        [SerializeField] private float cubeExpandDuration = 0.25f;
        [SerializeField] private float buttonStagger = 0.06f;
        [SerializeField] private float buttonPopDuration = 0.2f;

        Vector2 _restingPos;
        Sequence _activeSequence;

        void Awake()
        {
            panelRoot.pivot = new Vector2(0f, 0.5f);  
            _restingPos = panelRoot.anchoredPosition;
             // outerFrame/innerContent must always stay active — visibility is
            // controlled by scale, not SetActive, so tweens can always reach them.
            outerFrame.gameObject.SetActive(true);
            innerContent.gameObject.SetActive(true);
            foreach (var btn in buttons) btn.gameObject.SetActive(true);

            foreach (var btn in buttons) btn.localScale = Vector3.zero;
            panelRoot.gameObject.SetActive(false);
        }

        public void Open(Action onComplete = null)
        {
            Debug.Log("Open() called, panelRoot active before: " + panelRoot.gameObject.activeInHierarchy);

            _activeSequence.Stop();
            panelRoot.gameObject.SetActive(true);

            Debug.Log("panelRoot active after SetActive(true): " + panelRoot.gameObject.activeInHierarchy);

            panelRoot.anchoredPosition = _restingPos + offscreenOffset;
            panelRoot.localEulerAngles = new Vector3(0f, 90f, 0f);
            outerFrame.localScale = Vector3.zero;
            innerContent.localScale = Vector3.zero;
            foreach (var btn in buttons) btn.localScale = Vector3.zero;

            _activeSequence = Sequence.Create()
                .Group(Tween.UIAnchoredPosition(panelRoot, _restingPos, moveDuration, Ease.InExpo))
                .Chain(Tween.LocalEulerAngles(panelRoot, new Vector3(0, 90, 0), Vector3.zero, doorSwingDuration, Ease.OutCubic))
                .Group(Tween.Scale(outerFrame, Vector3.one, cubeExpandDuration, Ease.OutBack))
                .Insert(moveDuration + doorSwingDuration * 0.6f,
                    Tween.Scale(innerContent, Vector3.one, cubeExpandDuration * 0.8f, Ease.OutBack))
                .ChainCallback(this, target => target.AnimateButtonsIn())
                .ChainCallback(() => onComplete?.Invoke());
        }

        void AnimateButtonsIn()
        {
            var seq = Sequence.Create();
            for (int i = 0; i < buttons.Length; i++)
                seq.Insert(i * buttonStagger, Tween.Scale(buttons[i], Vector3.one, buttonPopDuration, Ease.OutBack));
        }

        public void Close(Action onComplete = null)
        {
            _activeSequence.Stop();

            var seq = Sequence.Create();
            for (int i = 0; i < buttons.Length; i++)
                seq.Insert(0f, Tween.Scale(buttons[i], Vector3.zero, 0.12f, Ease.InBack));

            seq
                .Chain(Tween.Scale(innerContent, Vector3.zero, cubeExpandDuration * 0.6f, Ease.InBack))
                .Group(Tween.Scale(outerFrame, Vector3.zero, cubeExpandDuration, Ease.InBack))
                .Group(Tween.LocalEulerAngles(panelRoot, Vector3.zero, new Vector3(0, 90, 0), doorSwingDuration, Ease.InCubic))
                .Chain(Tween.UIAnchoredPosition(panelRoot, _restingPos + offscreenOffset, moveDuration, Ease.OutExpo))
                .ChainCallback(this, target => target.panelRoot.gameObject.SetActive(false))  // was: target.gameObject.SetActive(false)
                .ChainCallback(() => onComplete?.Invoke());

            _activeSequence = seq;
        }
    }
}