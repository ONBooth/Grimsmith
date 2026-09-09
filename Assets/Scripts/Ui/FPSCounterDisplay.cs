using UnityEngine;
using TMPro;

namespace Games.UI
{
    public class FPSCounterDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fpsText;
        float deltaTime;

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f; // smoothed, avoids jittery per-frame numbers
            float fps = 1f / deltaTime;
            fpsText.text = $"{fps:0} FPS";
        }
    }
}