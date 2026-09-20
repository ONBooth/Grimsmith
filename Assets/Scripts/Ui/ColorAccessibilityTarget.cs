using System;
using UnityEngine;
using UnityEngine.UI;

namespace Games.UI
{
    [RequireComponent(typeof(Graphic))]
    public class ColorAccessibilityTarget : MonoBehaviour
    {
        [SerializeField] private Graphic graphic;
        [SerializeField] private bool participatesInRainbow = true;
        [SerializeField] private bool participatesInColorblindCorrection = true;

        Color baseColor;

        void Awake()
        {
            if (graphic == null) graphic = GetComponent<Graphic>();
            baseColor = graphic.color;
        }

        void OnEnable()
        {
            AccessibilityColorManager.Register(this);
            RefreshColor();
        }

        void OnDisable()
        {
            AccessibilityColorManager.Unregister(this);
        }

        public void RefreshColor()
        {
            if (graphic == null) return;
            Color result = baseColor;
            if (participatesInRainbow && AccessibilityColorManager.RainbowEnabled)
            {
                result = AccessibilityColorManager.GetRainbowColor(baseColor);
            }
            else if (participatesInColorblindCorrection && AccessibilityColorManager.CurrentMode !=
                     AccessibilityColorManager.ColorblindMode.None)
            {
                result = AccessibilityColorManager.ApplyColorblindCorrection(baseColor);
            }
            graphic.color = result;
        }

        public void SetColor(Color newColor)
        {
            baseColor = newColor;
            RefreshColor();
        }
    }
}