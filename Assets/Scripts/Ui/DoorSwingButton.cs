using UnityEngine;
using System.Collections;

namespace Games.UI
{
    /// <summary>
    /// Attach to each button. 
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class DoorSwingButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Position (anchoredPosition values)")]
        [Tooltip("Where the button sits while folded against the market board.")]
        [SerializeField] private Vector2 closedPosition;
        [Tooltip("Where the button ends up once fully open, to the right of the board.")]
        [SerializeField] private Vector2 openPosition;

        [Header("Swing Angle (Z rotation, degrees)")]
        [Tooltip("Rotation while closed/folded against the board, e.g. -90.")]
        [SerializeField] private float closedAngle = -90f;
        [Tooltip("Rotation once fully open, normally 0.")]
        [SerializeField] private float openAngle = 0f;

        [Header("Timing")]
        [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [Range(0.05f, 2f)] [SerializeField] private float duration = 0.4f;

        public bool IsOpen { get; private set; }

        private Coroutine _routine;

        private void Reset()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

            // Pivot must sit on the right edge so rotation hinges there.
            // Doing this via script (instead of just dragging it in the editor)
            // means it can't be forgotten and the button won't jump on screen.
            SetPivotPreservingPosition(rectTransform, new Vector2(1f, 0.5f));

            // Start folded/hidden against the board until told to open.
            rectTransform.anchoredPosition = closedPosition;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, closedAngle);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>Swing this button open, optionally after a delay (for staggering).</summary>
        public void PlayOpen(float delay = 0f)
        {
            Restart(OpenRoutine(delay));
        }

        /// <summary>Swing this button closed, optionally after a delay (for staggering).</summary>
        public void PlayClose(float delay = 0f)
        {
            Restart(CloseRoutine(delay));
        }

        private void Restart(IEnumerator routine)
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(routine);
        }

        private IEnumerator OpenRoutine(float delay)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            yield return Animate(closedPosition, openPosition, closedAngle, openAngle, 0f, 1f);

            IsOpen = true;
            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        private IEnumerator CloseRoutine(float delay)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            yield return Animate(openPosition, closedPosition, openAngle, closedAngle, 1f, 0f);

            IsOpen = false;
        }

        private IEnumerator Animate(Vector2 fromPos, Vector2 toPos, float fromAngle, float toAngle, float fromAlpha, float toAlpha)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = easingCurve.Evaluate(elapsed / duration);

                rectTransform.anchoredPosition = Vector2.LerpUnclamped(fromPos, toPos, t);
                float angle = Mathf.LerpUnclamped(fromAngle, toAngle, t);
                rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);

                if (canvasGroup != null)
                    canvasGroup.alpha = Mathf.LerpUnclamped(fromAlpha, toAlpha, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            rectTransform.anchoredPosition = toPos;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, toAngle);
            if (canvasGroup != null) canvasGroup.alpha = toAlpha;
        }

        /// <summary>Moves a RectTransform's pivot without letting it visually jump.</summary>
        private static void SetPivotPreservingPosition(RectTransform rt, Vector2 newPivot)
        {
            if (rt == null) return;
            Vector2 size = rt.rect.size;
            Vector2 deltaPivot = rt.pivot - newPivot;
            Vector2 deltaPosition = new Vector2(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rt.pivot = newPivot;
            rt.anchoredPosition += deltaPosition;
        }
    }
}
