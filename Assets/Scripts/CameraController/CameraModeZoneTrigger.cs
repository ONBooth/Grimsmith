using UnityEngine;

namespace Games.Camera
{
    public class CameraModeZoneTrigger : MonoBehaviour
    {
        [SerializeField] private CameraModeManager cameraModeManager;
        [SerializeField] private CameraModeManager.Mode modeOnEnter;
        [SerializeField] private CameraModeManager.Mode modeOnExit;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (modeOnEnter == CameraModeManager.Mode.ThreeD)
                cameraModeManager.SwitchTo3D();
            else
                cameraModeManager.SwitchTo2D();
        }
        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (modeOnExit == CameraModeManager.Mode.TwoD)
                cameraModeManager.SwitchTo2D();
            else
                cameraModeManager.SwitchTo3D();
        }
    }

}