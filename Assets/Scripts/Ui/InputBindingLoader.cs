using UnityEngine;
using UnityEngine.InputSystem;

namespace Games.UI
{
    /// <summary>
    /// Put on your persistent "Managers" object with a reference to your
    /// PlayerControls InputActionAsset. Runs once at startup and re-applies
    /// any button layout changes the player saved via RebindActionUI.
    /// </summary>
    public class InputBindingLoader : MonoBehaviour
    {
        [SerializeField] private InputActionAsset actionAsset;

        private static bool _loaded;

        private void Awake()
        {
            if (_loaded) { Destroy(gameObject); return; } // only need to run once, on first load
            _loaded = true;

            string overrides = PlayerPrefs.GetString("input_binding_overrides", string.Empty);
            if (!string.IsNullOrEmpty(overrides))
                actionAsset.LoadBindingOverridesFromJson(overrides);
        }
    }
}