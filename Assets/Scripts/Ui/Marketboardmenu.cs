
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Games.UI
{
    /// <summary>
    /// Drives a set of DoorSwingButton items so they open/close one after
    /// another, cascading off the market board.
    /// </summary>
    public class MarketBoardMenu : MonoBehaviour
    {
        [Tooltip("Buttons in the order you want them to cascade open.")]
        [SerializeField] private List<DoorSwingButton> buttons = new List<DoorSwingButton>();

        [Tooltip("Delay added between each successive button opening/closing.")]
        [Range(0f, 0.5f)] [SerializeField] private float staggerDelay = 0.08f;

        [Tooltip("If true, buttons close in reverse order (last one opened closes first).")]
        [SerializeField] private bool reverseOnClose = true;

        public static event Action OnOpenBoard;
        public static event Action OnCloseBoard;

        private bool _isOpen;

        [ContextMenu("Open Board")]
        public void OpenBoard()
        {
            if (_isOpen) return;
            _isOpen = true;
            OnOpenBoard?.Invoke();

            for (int i = 0; i < buttons.Count; i++)
                buttons[i].PlayOpen(i * staggerDelay);
        }

        [ContextMenu("Close Board")]
        public void CloseBoard()
        {
            if (!_isOpen) return;
            _isOpen = false;
            OnCloseBoard?.Invoke();

            int count = buttons.Count;
            for (int i = 0; i < count; i++)
            {
                int order = reverseOnClose ? (count - 1 - i) : i;
                buttons[i].PlayClose(order * staggerDelay);
            }
        }

        [ContextMenu("Toggle Board")]
        public void ToggleBoard()
        {
            if (_isOpen) CloseBoard();
            else OpenBoard();
        }
    }
}