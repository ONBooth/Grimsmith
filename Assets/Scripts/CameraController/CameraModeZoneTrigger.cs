using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Games.Camera
{
    public class CameraModeZoneTrigger : MonoBehaviour
    {
        [SerializeField] private CameraModeManager cameraModeManager;
        [SerializeField] private CameraModeManager.Mode modeOnEnter;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (modeOnEnter == CameraModeManager.Mode.ThreeD)
                cameraModeManager.SwitchTo3D();
            else
                cameraModeManager.SwitchTo2D();
        }
    }
}