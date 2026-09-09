using UnityEngine; 
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.PlayerLoop;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Games.UI
{
    public class MenuUi : MonoBehaviour
    {
        [Header("Panel Settings")] 
        [SerializeField] private GameObject panel;

        [SerializeField] private RectTransform panelRectTransform;
        [SerializeField] private CanvasGroup panelCanvasGroup;

        public enum AnimateToDirection
        {
            Top,
            Bottom,
            Left,
            Right
        }

        [Header("Animation Settings")] 
        [SerializeField] private AnimateToDirection openDirection = AnimateToDirection.Top;
        [SerializeField] private AnimateToDirection closeDirection = AnimateToDirection.Bottom;
        [SerializeField] private Vector2 distanceToAnimate = new Vector2(x: 100, y: 100);
        [Space]
        [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, valueStart: 0, 1, valueEnd: 1);
        [Range(0, 1f)] [SerializeField] private float animationDuration = 0.5f;

        private bool _isOpen;
        private Vector2 _initialPosition;
        private Vector2 _currentPosition;

        private Vector2 _upOffset;
        private Vector2 _downOffset;
        private Vector2 _leftOffset;
        private Vector2 _rightOffset;

        private Coroutine _animatePanelCoroutine;

        [Header("ChangeHelpers")] 
        [SerializeField] private bool changeGizmos = true;

        public static event Action OnOpenPanel;
        public static event Action OnClosePanel;

        private enum DisplayGizmosAtLocation
        {
            Open,
            Close,
            Both,
            Situational,
        }

        [SerializeField ] private DisplayGizmosAtLocation gizmoHandler;

        [SerializeField ] private Color gizmoOpenColor = Color.green;

        [SerializeField ] private Color gizmoCloseColor = Color.red;

        [SerializeField ] private Color gizmoInitallocationColor = Color.grey;

        private Vector2 _panelOpenPositionForGizmo;
        private Vector2 _panelClosePositionForGizmo;
        private Vector2 _initialPositionForGizmos;





        private void OnValidate()

        {
            if (panel != null)
            {
                panelRectTransform = panel.GetComponent<RectTransform>();
                panelCanvasGroup = panel.GetComponent<CanvasGroup>();
            }

            distanceToAnimate.x = Mathf.Max(a: 0, b: distanceToAnimate.x);
            distanceToAnimate.y = Mathf.Max(a: 0, b: distanceToAnimate.y);

            _initialPosition = panel.transform.position;

            RecalculateGizmoPositions();

        }

        #region AnimateFunctionality

        private void Start()
        {
            _initialPosition = panel.transform.position;

            InitializeOffsetPostions();
        }

        private void InitializeOffsetPostions()
        {
            _upOffset = new Vector2(x: 0, distanceToAnimate.y);
            _downOffset = new Vector2(x: 0, -distanceToAnimate.y);

            _rightOffset = new Vector2(+distanceToAnimate.x, y: 0);
            _leftOffset = new Vector2(-distanceToAnimate.x, y: 0);
        }

        [ContextMenu(itemName: "Toggle Open Close")]
        public void ToggleOpenClose()
        {
            if (_isOpen)
                ClosePanel();
            else
            
                OpenPanel();
            
        }

        [ContextMenu("Open Panel")]
        public void OpenPanel()
        {
            if (_isOpen)
                return;
            _isOpen = true;
            OnOpenPanel?.Invoke();

            if (_animatePanelCoroutine != null)
                StopCoroutine(_animatePanelCoroutine);
            _animatePanelCoroutine = StartCoroutine(routine: AnimatePanel(open: true));
        }
        [ContextMenu("Close Panel")]
        public void ClosePanel()
        {
            if (!_isOpen)
                return;
            _isOpen = false;
            OnClosePanel?.Invoke();
            if (_animatePanelCoroutine != null)
                StopCoroutine(_animatePanelCoroutine);
            _animatePanelCoroutine = StartCoroutine(routine: AnimatePanel(open:false));
        }
        
        private Vector2 GetOffset(AnimateToDirection direction)
        {
            switch (direction)
            {
                case AnimateToDirection.Top:
                    return _upOffset;
                case AnimateToDirection.Bottom:
                    return _downOffset;
                case AnimateToDirection.Left:
                    return _leftOffset;
                case AnimateToDirection.Right:
                    return _rightOffset;
                default:
                    return Vector3.zero;
            }



        }

        private IEnumerator AnimatePanel(bool open)
        {
            if (open)
                panel.gameObject.SetActive(true);
            _currentPosition = panel.transform.position;
            float elapsedTime = 0;
            Vector2 targetPosition = _currentPosition;
            
            if (open)
                targetPosition = _currentPosition + GetOffset(openDirection);
            else
            targetPosition = _currentPosition + GetOffset(closeDirection);
            while (elapsedTime < animationDuration)    
            {
                float evaluationAtTime =easingCurve.Evaluate(time:elapsedTime/ animationDuration);
                panel.transform.position = Vector2.Lerp(a: _currentPosition, b: targetPosition, evaluationAtTime);

                panelCanvasGroup.alpha = open
                    ? Mathf.Lerp(a: 0f, b: 1f, evaluationAtTime)
                    : Mathf.Lerp(a: 1f, b: 0f, t:evaluationAtTime);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            panel.transform.position = targetPosition;

            panelCanvasGroup.alpha = open ? 1 : 0;
            panelCanvasGroup.interactable = open;
            panelCanvasGroup.blocksRaycasts = open;

            if (!open)
            {
                panel.gameObject.SetActive(false);
                panel.transform.position = _initialPosition;
            }

        }
#endregion
        #region Visualisation
        [ContextMenu("Refresh Visualisation")]

        private void Refresh()
        {
            OnValidate();
        }

        private void RecalculateGizmoPositions()
        {
            InitializeOffsetPostions();
            _initialPositionForGizmos = new Vector2(panel.transform.position.x, panel.transform.position.y) + panelRectTransform.rect.center;
            _panelOpenPositionForGizmo =  _initialPositionForGizmos + GetOffset(openDirection);
            _panelClosePositionForGizmo = _panelOpenPositionForGizmo + GetOffset(closeDirection);
        }

        private void OnDrawGizmos()
        {
            if (!changeGizmos)
                return;
            if (panel == null)
                return;
            if (panelRectTransform == null)
                return;
            Gizmos.color = gizmoInitallocationColor;
            Gizmos.DrawWireCube(center: _initialPositionForGizmos, panelRectTransform.sizeDelta);

            switch (gizmoHandler)
            {
                case DisplayGizmosAtLocation.Open:
                    DrawCube(_panelOpenPositionForGizmo,opens:true);
                    break;
                case DisplayGizmosAtLocation.Close:
                DrawCube(_panelClosePositionForGizmo,opens:false);
                    break;
                case  DisplayGizmosAtLocation.Both:
                    DrawCube(_panelClosePositionForGizmo, opens:true);
                    DrawCube(_panelOpenPositionForGizmo,opens:false);
                    break;
                
                default:
                    case DisplayGizmosAtLocation.Situational:
                        if (_isOpen)
                        DrawCube(_panelClosePositionForGizmo, opens:true);
                        else
                        DrawCube(_panelOpenPositionForGizmo, opens:false);
                    break;
            }
            DrawIndicators();
            }
                
                
            
            private void DrawCube(Vector2 panelPosition, bool opens)
            {
                Gizmos.color = opens ? gizmoOpenColor : gizmoCloseColor;
                Gizmos.DrawWireCube(center: panelPosition, panelRectTransform.sizeDelta);
            }
            
            private void DrawIndicators()
            {
                Gizmos.color = gizmoOpenColor;
                Gizmos.DrawLine(_initialPositionForGizmos, _panelClosePositionForGizmo);
                
                Gizmos.color = gizmoCloseColor;
                Gizmos.DrawLine(_panelOpenPositionForGizmo, _panelClosePositionForGizmo);
                
        }

             

        #endregion
    }
    
}


