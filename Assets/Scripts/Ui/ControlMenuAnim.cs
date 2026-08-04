using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System.Collections.Generic;

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

        [Header("Reset All")]
        [SerializeField] private Button resetAllButton;
        [SerializeField] private List<RebindActionUI> allRebindrows;
        [SerializeField] private float resetStaggerPerRow = 0.035f;

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
            resetAllButton.onClick.AddListener(ResetAll);
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

        // PrimeTween doesn't allow stopping an individual tween once it's been added
        // to a Sequence (it becomes "nested" and only the parent Sequence can stop it).
        // Since the background punch animation is built with Sequence.Create().Chain(...),
        // we track that Sequence directly and stop IT, rather than calling
        // Tween.StopAll(onTarget: background) — which would try to stop the nested
        // tweens individually and throw.
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

        public void ResetAll()
        {
            // Ignore extra clicks while a reset wave is already playing out — without
            // this, spamming the button queues overlapping batches of Tween.Delay
            // callbacks, and if a row gets deactivated (e.g. from switching tabs)
            // before its delayed callback fires, that callback throws trying to
            // animate a transform that's no longer valid.
            if (isResetting) return;
            isResetting = true;
            resetAllButton.interactable = false;

            // Punch the button itself for immediate feedback on click.
            Tween.Scale(resetAllButton.transform, 1.2f, 0.15f, Ease.OutSine, cycles: 2, cycleMode: CycleMode.Yoyo);

            // Extra background pulse to sell "something big just happened".
            StopBackgroundAnimations();
            backgroundPunchSequence = Sequence.Create()
                .Chain(Tween.Scale(background, switchZoomPunchScale * 1.02f, 0.25f, Ease.OutSine))
                .Chain(Tween.Scale(background, 1f, 0.4f, Ease.InOutSine))
                .OnComplete(() => StartAmbientZoom());

            float lastDelay = 0f;

            for (var i = 0; i < allRebindrows.Count; i++)
            {
                var row = allRebindrows[i];
                if (row == null) continue;

                var delay = i * resetStaggerPerRow;
                lastDelay = delay;

                Tween.Delay(delay, () =>
                {
                    // Row may have been destroyed or deactivated between queuing this
                    // callback and it actually firing — bail out safely instead of
                    // throwing on a stale reference.
                    if (row == null) return;

                    row.ResetToDefault();
                    Tween.Scale(row.transform, 1.15f, 0.18f, Ease.OutBack, cycles: 2, cycleMode: CycleMode.Yoyo);
                });
            }

            // Re-enable the button once the full staggered wave has finished playing.
            Tween.Delay(lastDelay + 0.25f, () =>
            {
                isResetting = false;
                resetAllButton.interactable = true;
            });
        }
    }
}