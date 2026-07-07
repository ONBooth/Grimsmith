using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Games.UI
{
    /// <summary>
    /// Attach to a UI button that rebinds one action's binding.
    /// bindingIndex is the index into action.bindings — for a simple
    /// action with one Keyboard binding and one Gamepad binding, index 0
    /// is usually keyboard and index 1 is usually gamepad (check the
    /// order in your PlayerControls asset). Make one of these per
    /// button per binding you want rebindable.
    /// </summary>
    public class RebindActionUI : MonoBehaviour
    {
        [SerializeField] private InputActionReference actionReference;
        [SerializeField] private int bindingIndex = 0;
        [SerializeField] private Text bindingDisplayText; // swap for TMP_Text if using TextMeshPro
        [SerializeField] private GameObject rebindPrompt;  // optional "Press any key..." popup

        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

        private void OnEnable()
        {
            RefreshDisplayText();
        }

        public void StartRebind()
        {
            var action = actionReference.action;
            action.Disable();

            if (rebindPrompt != null) rebindPrompt.SetActive(true);

            _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation =>
                {
                    action.Enable();
                    if (rebindPrompt != null) rebindPrompt.SetActive(false);
                    RefreshDisplayText();
                    SaveOverrides(action);
                    operation.Dispose();
                })
                .OnCancel(operation =>
                {
                    action.Enable();
                    if (rebindPrompt != null) rebindPrompt.SetActive(false);
                    operation.Dispose();
                })
                .Start();
        }

        public void ResetToDefault()
        {
            var action = actionReference.action;
            action.RemoveBindingOverride(bindingIndex);
            RefreshDisplayText();
            SaveOverrides(action);
        }

        private void RefreshDisplayText()
        {
            if (bindingDisplayText == null || actionReference == null) return;
            bindingDisplayText.text = actionReference.action.GetBindingDisplayString(bindingIndex);
        }

        private static void SaveOverrides(InputAction action)
        {
            string json = action.actionMap.asset.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("input_binding_overrides", json);
        }
    }
}
