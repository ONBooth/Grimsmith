using UnityEngine;

namespace Games.Camera
{
    public static class PlayerMovementPlane
    {
        public static CameraModeManager.Mode CurrentMode { get; private set; } = CameraModeManager.Mode.TwoD;

        public static void SetMode(CameraModeManager.Mode mode) => CurrentMode = mode;

        // Call this from your PlayerController's OnMove, instead of using the raw
        // Vector2 directly, so movement is correctly flattened in 2D mode.
        public static Vector3 ConstrainMovement(Vector2 rawInput, Transform cameraTransform)
        {
            if (CurrentMode == CameraModeManager.Mode.TwoD)
            {
                // Lock to X/Y (or X/Z depending on your 2D plane's orientation) — ignore depth entirely.
                return new Vector3(rawInput.x, rawInput.y, 0f);
            }

            // 3D mode: standard camera-relative movement.
            Vector3 forward = cameraTransform.forward; forward.y = 0; forward.Normalize();
            Vector3 right = cameraTransform.right; right.y = 0; right.Normalize();
            return forward * rawInput.y + right * rawInput.x;
        }
    }
}