using UnityEngine;
using System;
using PrimeTween;

namespace  Game.UI
{
    
}

public class PausePanelAnimator : MonoBehaviour
{
    [Header("Panel Parts")] 
    [SerializeField] private RectTransform pausePanel;
    [SerializeField] private RectTransform panelTextBox;

    [Header("Panel Speed")] [SerializeField]
    private Vector2 offscreenOffset = new Vector2(-900, 0f);

    [SerializeField] private float popInDuration = 0.16f;
    [SerializeField] private float settleDuration = 0.35f;
    [SerializeField] private float landingShakeDuration = 0.35f;

    [Header("Text Box Speed")] [SerializeField]
    private Vector2 textHiddenOffset = new Vector2(0f, 45f);

    [SerializeField] private float textStartDelay = 0.15f;
    [SerializeField] private float textSlideDuration = 0.15f;

    Vector2 _panelRestingPos;
    Vector2 _textRestingPos;
    Sequence _activeSequence;

    private void Awake()
    {
        _panelRestingPos = pausePanel.anchoredPosition;
        _textRestingPos = panelTextBox.anchoredPosition;

        panelTextBox.gameObject.SetActive(true); // active always - stays hidden via scale 
        panelTextBox.localScale = Vector3.zero;

        pausePanel.gameObject.SetActive(false);
    }

    public void Open(Action onComplete = null)
    {
        _activeSequence.Stop();
        Time.timeScale = 0f; // pause game
        pausePanel.gameObject.SetActive(true);

          // reset position tp closed instead of open state
        pausePanel.anchoredPosition = _panelRestingPos + offscreenOffset;
        pausePanel.localScale = new Vector3(0.4f, 1.3f, 1f);
        pausePanel.localEulerAngles = new Vector3(0f, 0f, -12f);

        panelTextBox.anchoredPosition = _textRestingPos + textHiddenOffset;
        panelTextBox.localScale = Vector3.zero;
        panelTextBox.localEulerAngles = new Vector3(0f, 0f, -8f);
        
        _activeSequence = Sequence.Create(useUnscaledTime: true)
            
            .Group(Tween.UIAnchoredPosition(pausePanel, _panelRestingPos, popInDuration, Ease.OutExpo, useUnscaledTime: true))
            .Group(Tween.LocalEulerAngles(pausePanel, new Vector3(0, 0, -12f), Vector3.zero, popInDuration, Ease.OutExpo, useUnscaledTime: true))

                .Chain(Tween.Scale(pausePanel, Vector3.one, settleDuration, Ease.OutElastic, useUnscaledTime: true))
                .Group(Tween.ShakeLocalPosition(pausePanel, strength: new Vector3(10f, 5f, 0f),
                    duration: landingShakeDuration, frequency: 12, useUnscaledTime: true))
            
                .InsertCallback(popInDuration + textStartDelay, () => { })
                .Insert(popInDuration + textStartDelay,
                    Tween.UIAnchoredPosition(panelTextBox, _textRestingPos, textSlideDuration, Ease.OutQuint, useUnscaledTime: true))
                .Insert(popInDuration + textStartDelay,
                    Tween.Scale(panelTextBox, Vector3.one, textSlideDuration, Ease.OutBack, useUnscaledTime: true))
                .Insert(popInDuration + textStartDelay,
                    Tween.LocalEulerAngles(panelTextBox, new Vector3(0, 0, -8f), Vector3.zero, textSlideDuration * 0.7f, Ease.OutBack, useUnscaledTime: true))

                .ChainCallback(() => onComplete?.Invoke());
        }

        public void Close(Action onComplete = null)
        {
            _activeSequence.Stop();

            _activeSequence = Sequence.Create(useUnscaledTime: true)
               
                .Group(Tween.Scale(panelTextBox, Vector3.zero, 0.15f, Ease.InBack, useUnscaledTime: true))
                .Group(Tween.UIAnchoredPosition(panelTextBox, _textRestingPos + textHiddenOffset, 0.15f, Ease.InQuint, useUnscaledTime: true))

               
                .Chain(Tween.Scale(pausePanel, new Vector3(0.4f, 1.3f, 1f), 0.16f, Ease.InBack, useUnscaledTime: true))
                .Group(Tween.LocalEulerAngles(pausePanel, Vector3.zero, new Vector3(0, 0, 14f), 0.16f, Ease.InBack, useUnscaledTime: true))
                .Chain(Tween.UIAnchoredPosition(pausePanel, _panelRestingPos + offscreenOffset, popInDuration, Ease.InExpo, useUnscaledTime: true))

                .ChainCallback(this, target => target.pausePanel.gameObject.SetActive(false))
                .ChainCallback(() =>
                {
                    Time.timeScale = 1f; // resume gameplay
                    onComplete?.Invoke();
                });
    }
}