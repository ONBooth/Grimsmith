using UnityEngine;
using Unity.Cinemachine;

namespace Games.Camera
{
    // ============================================================================
    // CAMERA MODE MANAGER
    // Switches between a flat "2D" CinemachineCamera and a full "3D" one by
    // adjusting Priority — the higher-priority camera becomes Live, and
    // CinemachineBrain (on the Main Camera) handles the actual blend automatically.
    // ============================================================================
    public class CameraModeManager : MonoBehaviour
    {
        public enum Mode { TwoD, ThreeD }

        [SerializeField] private CinemachineCamera camera2D;
        [SerializeField] private CinemachineCamera camera3D;

        const int ActivePriority = 20;
        const int InactivePriority = 0; // avoid equal priorities — ties make the Brain's pick unstable

        public Mode CurrentMode { get; private set; } = Mode.TwoD;

        void Start()
        {
            ApplyMode(Mode.TwoD, instant: true);
        }

        public void SwitchTo2D() => ApplyMode(Mode.TwoD);
        public void SwitchTo3D() => ApplyMode(Mode.ThreeD);
        public void Toggle() => ApplyMode(CurrentMode == Mode.TwoD ? Mode.ThreeD : Mode.TwoD);

        void ApplyMode(Mode mode, bool instant = false)
        {
            CurrentMode = mode;

            camera2D.Priority = mode == Mode.TwoD ? ActivePriority : InactivePriority;
            camera3D.Priority = mode == Mode.ThreeD ? ActivePriority : InactivePriority;

            // Notify the player controller so movement input maps correctly
            // to whichever plane/axes are currently relevant.
            PlayerMovementPlane.SetMode(mode);
        }
    }
}