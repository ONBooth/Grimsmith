using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System.Collections.Generic;
using TMPro;

namespace Games.UI
{
    // Keyboard/Controller settings screen
    // Switching tabs animates the old panel out and the new one in

    public class ControlMenuAnim : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private RectTransform keyboardPanel;
        [SerializeField] private CanvasGroup keyboardCanvasGroup;
        [SerializeField] private RectTransform controllerPanel;
        [SerializeField] private CanvasGroup controllerCanvasGroup;

        [Header("Buttons")]
        [SerializeField] private Button keyboardButton;
        [SerializeField] private Button controllerButton;

        [Tooltip("Image that slides under whichever tab is active. Leave empty to skip.")]
        [SerializeField] private RectTransform Underline;

        [Header("Background")]
        [Tooltip("Fullscreen background image")]
        [SerializeField] private RectTransform background;

        [SerializeField] private float ambientZoomScale = 1.06f;
        [SerializeField] private float ambientZoomDuration = 6f;
        [SerializeField] private float switchZoomPunchScale = 1.1f;
        [SerializeField] private float switchZoomPunchDuration = 0.6f;
        
        
        [Header("Confirm / Revert Safety Net")]
        [SerializeField] private GameObject WaitForInputPanel;
        [SerializeField] private TextMeshProUGUI WaitForInputText;
        [SerializeField] private float revertTimeoutSeconds = 8f;
        Coroutine revertCoroutine;
        
        [Header("Timing")]
        [SerializeField] private float panelSwitchDuration = 0.35f;
        [SerializeField] private float panelSlideDistance = 70f;

        private bool showingKeyboard = true;
        private Tween ambientZoomTween;
        private Sequence backgroundPunchSequence;
        private bool isResetting;

        private void Awake()
        {
            keyboardButton.onClick.AddListener(() => SwitchTo(true));
            controllerButton.onClick.AddListener(() => SwitchTo(false));
            
        }

        private void OnEnable()
        {
            keyboardPanel.gameObject.SetActive(showingKeyboard);
            controllerPanel.gameObject.SetActive(!showingKeyboard);
            keyboardCanvasGroup.alpha = showingKeyboard ? 1f : 0f;
            controllerCanvasGroup.alpha = showingKeyboard ? 0f : 1f;
            keyboardPanel.anchoredPosition = Vector2.zero;
            controllerPanel.anchoredPosition = Vector2.zero;

            if (Underline != null)
                SnapUnderline(showingKeyboard);

            StartAmbientZoom();
            
        }

        private void OnDisable()
        {
            StopBackgroundAnimations();
        }

        
        private void StopBackgroundAnimations()
        {
            ambientZoomTween.Stop();
            backgroundPunchSequence.Stop();
        }

        private void StartAmbientZoom()
        {
            background.localScale = Vector3.one;
            ambientZoomTween = Tween.Scale(background, endValue: ambientZoomScale, duration: ambientZoomDuration,
                ease: Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo);
        }

        private void SwitchTo(bool keyboard)
        {
            if (keyboard == showingKeyboard) return;
            showingKeyboard = keyboard;

            // NOTE: these must be the PANELS (RectTransform), not the buttons —
            // the buttons just triggered this method, they aren't what moves.
            var enterPanel = keyboard ? keyboardPanel : controllerPanel;
            var exitPanel = keyboard ? controllerPanel : keyboardPanel;
            var enterGroup = keyboard ? keyboardCanvasGroup : controllerCanvasGroup;
            var exitGroup = keyboard ? controllerCanvasGroup : keyboardCanvasGroup;

            float direction = keyboard ? -1f : 1f;

            enterPanel.gameObject.SetActive(true);
            enterPanel.anchoredPosition = new Vector2(-direction * panelSlideDistance, 0f);
            enterGroup.alpha = 0f;

            var exitTargetPos = new Vector2(direction * panelSlideDistance, 0f);

            Sequence.Create()
                .Group(Tween.Alpha(exitGroup, 0f, panelSwitchDuration * 0.6f, Ease.InSine))
                .Group(Tween.UIAnchoredPosition(exitPanel, exitTargetPos, panelSwitchDuration * 0.6f, Ease.InSine))
                .Group(Tween.Alpha(enterGroup, 1f, panelSwitchDuration, Ease.OutCubic))
                .Group(Tween.UIAnchoredPosition(enterPanel, Vector2.zero, panelSwitchDuration, Ease.OutCubic))
                .OnComplete(() => exitPanel.gameObject.SetActive(false));

            if (Underline != null)
                SlideUnderline(keyboard);

            // Quick background zoom "punch" as feedback for the tab press, then resume
            // the ambient loop once it settles back to normal scale.
            StopBackgroundAnimations();
            backgroundPunchSequence = Sequence.Create()
                .Chain(Tween.Scale(background, switchZoomPunchScale, switchZoomPunchDuration * 0.4f, Ease.OutSine))
                .Chain(Tween.Scale(background, 1f, switchZoomPunchDuration * 0.6f, Ease.InOutSine))
                .OnComplete(() => StartAmbientZoom());

            // Small pop on the tab that was just pressed.
            var pressedTab = (keyboard ? keyboardButton : controllerButton).transform;
            Tween.Scale(pressedTab, 1.12f, 0.12f, Ease.OutSine, cycles: 2, cycleMode: CycleMode.Yoyo);
        }

        // Instantly places the underline under the given tab, no animation —
        // used on OnEnable so the menu doesn't animate the underline sliding
        // in from nowhere the very first time it's shown.
        private void SnapUnderline(bool keyboard)
        {
            var targetRect = (RectTransform)(keyboard ? keyboardButton : controllerButton).transform;
            Underline.position = new Vector3(targetRect.position.x, Underline.position.y, Underline.position.z);
        }

        // Animates the underline sliding to sit under the newly active tab.
        private void SlideUnderline(bool keyboard)
        {
            var targetRect = (RectTransform)(keyboard ? keyboardButton : controllerButton).transform;
            Tween.PositionX(Underline, targetRect.position.x, panelSwitchDuration, Ease.OutCubic);
        
        }
        void BeginConfirmCountdown()
        {
            if (revertCoroutine != null) StopCoroutine(revertCoroutine);
            revertCoroutine = StartCoroutine(ConfirmCountdownRoutine());
        }

        IEnumerator ConfirmCountdownRoutine()
        {
            WaitForInputPanel.SetActive(true);
            float remaining = revertTimeoutSeconds;

            while (remaining > 0f)
            {
                WaitForInputText.text = $"Reverting in {Mathf.CeilToInt(remaining)}...";
                yield return null;
                remaining -= Time.unscaledDeltaTime; // unscaled — this must work even if the game is paused
            }

            RevertNow();
        }

        public void KeepChanges()
        {
            if (revertCoroutine != null) StopCoroutine(revertCoroutine);
            WaitForInputPanel.SetActive(false);
        }

        public void RevertNow()
        {
            if (revertCoroutine != null) StopCoroutine(revertCoroutine);
            WaitForInputPanel.SetActive(false);

            // Refresh dropdowns to reflect the restored state.
            
        }
    }
}