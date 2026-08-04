using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

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
        [Header("Action to rebind")]
        // Reference to the .inputactions asset's action this row controls (e.g. "Move").
        [SerializeField] private InputActionReference action;

        // The specific binding within that action this row edits. For a simple action
        // (like Jump) there's only one binding. For a composite (like Move's Up/Down/
        // Left/Right), each direction is its own binding with its own ID — this field
        // tells the script exactly which part this row is responsible for.
        [SerializeField] private string bindingId;

        // Controls how the fallback text is formatted (e.g. include/exclude modifiers).
        [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;

        [Header("UI References")]
        [SerializeField] private TMP_Text actionLabel;       // shows the action's name, e.g. "Move Up"
        [SerializeField] private TMP_Text bindingText;        // fallback text label if no icon exists
        [SerializeField] private Image bindingIcon;            // icon shown instead of text when available
        [SerializeField] private Button rebindButton;          // the button the player clicks to start rebinding
        [SerializeField] private TMP_Text rebindButtonText;    // "Press a key..." prompt shown while waiting
        [SerializeField] private GameObject rebindOverlay;     // optional full-screen blocker while waiting

        [Header("Icons (optional — leave empty to always use text)")]
        [SerializeField] private InputIconDatabase iconDatabase;

        [Header("Conflict Warning")]
        [SerializeField] private TMP_Text conflictWarningText;     // shared warning label, e.g. "Already used by Down!"
        [SerializeField] private float conflictWarningDuration = 2f;

        // The in-progress rebind listener. Null whenever we're not actively waiting for input.
        RebindingOperation rebindOperation;

        // --- Small helpers to resolve the actual InputAction / binding index from the
        // Inspector references, since InputActionReference + a string GUID isn't
        // directly usable — it needs to be looked up each time. ---

        InputAction ResolveAction() => action?.action;

        int ResolveBindingIndex(InputAction resolvedAction)
        {
            var id = new Guid(bindingId);
            for (int i = 0; i < resolvedAction.bindings.Count; i++)
            {
                if (resolvedAction.bindings[i].id == id)
                    return i;
            }
            return -1; // not found — bindingId probably wasn't set correctly in the Inspector
        }

        void OnEnable()
        {
            UpdateActionLabel();
            UpdateBindingDisplay();
        }

        void UpdateActionLabel()
        {
            if (actionLabel != null)
                actionLabel.text = action?.action?.name ?? string.Empty;
        }

        // Refreshes what's shown for the current binding — called on enable, and again
        // every time a rebind successfully completes, so the row always reflects reality.
        void UpdateBindingDisplay()
        {
            var resolvedAction = ResolveAction();
            if (resolvedAction == null) return;

            var bindingIndex = ResolveBindingIndex(resolvedAction);
            if (bindingIndex < 0) return;

            string controlPath = resolvedAction.bindings[bindingIndex].effectivePath;
            string deviceLayoutName = InferDeviceLayoutName(controlPath);

            Sprite icon = iconDatabase != null ? iconDatabase.GetIcon(controlPath, deviceLayoutName) : null;

            // Icon found: show the icon, hide the text.
            // No icon: fall back to Unity's built-in human-readable text (e.g. "W", "Left Shoulder").
            if (icon != null)
            {
                if (bindingIcon != null) { bindingIcon.sprite = icon; bindingIcon.enabled = true; }
                if (bindingText != null) bindingText.gameObject.SetActive(false);
            }
            else
            {
                if (bindingIcon != null) bindingIcon.enabled = false;
                if (bindingText != null)
                {
                    bindingText.gameObject.SetActive(true);
                    bindingText.text = resolvedAction.GetBindingDisplayString(bindingIndex, displayStringOptions);
                }
            }
        }

        // Best-effort guess at which controller layout a path belongs to, purely so the
        // icon database can pick between e.g. Xbox vs PlayStation glyphs for buttonSouth.
        // Extend this if you support more controller types.
        static string InferDeviceLayoutName(string controlPath)
        {
            if (controlPath.Contains("XInputController")) return "XInputController";
            if (controlPath.Contains("DualShockGamepad")) return "DualShockGamepad";
            if (controlPath.Contains("Gamepad")) return "Gamepad"; // generic fallback
            if (controlPath.Contains("Keyboard")) return "Keyboard";
            return string.Empty;
        }

        /// <summary>
        /// Wire this to the row's Button OnClick() in the Inspector.
        /// Starts listening for the next input the player presses.
        /// </summary>
        public void StartInteractiveRebind()
        {
            var resolvedAction = ResolveAction();
            if (resolvedAction == null) return;

            var bindingIndex = ResolveBindingIndex(resolvedAction);
            if (bindingIndex < 0) return;

            PerformInteractiveRebind(resolvedAction, bindingIndex);
        }

        // allCompositeParts: when true, after this part finishes rebinding, automatically
        // moves on to the NEXT part of the same composite (e.g. finish Up, then prompt for
        // Down). Pass true only from the FIRST row of a composite if you want a guided
        // "rebind all four directions in a row" flow; leave false for independent single rows.
        void PerformInteractiveRebind(InputAction actionToRebind, int bindingIndex, bool allCompositeParts = false)
        {
            // Cancel any rebind already in progress before starting a new one.
            rebindOperation?.Cancel();

            // Unity's Input System throws InvalidOperationException if you try to
            // rebind an action while it's still enabled — which it normally is,
            // since the player needs it active to navigate the menu in the first
            // place. Disable it for the duration of the rebind, then restore
            // whatever its enabled state was before we touched it.
            var wasEnabled = actionToRebind.enabled;
            if (wasEnabled)
                actionToRebind.Disable();

            void CleanUp()
            {
                rebindOperation?.Dispose();
                rebindOperation = null;

                if (wasEnabled)
                    actionToRebind.Enable();
            }

            if (rebindButtonText != null) rebindButtonText.gameObject.SetActive(false);
            rebindOverlay?.SetActive(true);

            rebindOperation = actionToRebind.PerformInteractiveRebinding(bindingIndex)
                .WithActionEventNotificationsBeingSuppressed()

                // Player pressed Escape or the operation was cancelled some other way.
                .OnCancel(operation =>
                {
                    if (rebindButtonText != null) rebindButtonText.gameObject.SetActive(true);
                    rebindOverlay?.SetActive(false);
                    CleanUp();
                })

                // Fires the MOMENT a candidate input is detected, BEFORE it's actually
                // written to the binding. This is the hook that makes conflict-checking
                // possible: we can inspect "path" and simply not call ApplyBindingOverride
                // if it's already used elsewhere, which rejects the rebind cleanly with
                // zero flicker (nothing was ever actually overwritten).
                .OnApplyBinding((operation, path) =>
                {
                    if (CheckDuplicateBindings(actionToRebind, bindingIndex, path, allCompositeParts, out string conflictingActionName))
                    {
                        ShowConflictMessage(conflictingActionName);
                        return; // do NOT apply — rebind is rejected
                    }

                    actionToRebind.ApplyBindingOverride(bindingIndex, path);
                })

                // Fires after OnApplyBinding, whether or not it actually applied anything.
                .OnComplete(operation =>
                {
                    if (rebindButtonText != null) rebindButtonText.gameObject.SetActive(true);
                    rebindOverlay?.SetActive(false);
                    UpdateBindingDisplay(); // refresh text/icon to reflect the (possibly unchanged) binding
                    CleanUp();

                    // Guided multi-part flow: automatically continue to the next direction.
                    if (allCompositeParts)
                    {
                        var nextBindingIndex = bindingIndex + 1;
                        if (nextBindingIndex < actionToRebind.bindings.Count &&
                            actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                            PerformInteractiveRebind(actionToRebind, nextBindingIndex, true);
                    }
                });

            rebindOperation.Start();
        }

        // Checks whether "newPath" (the control the player just pressed) is already used
        // by ANY other binding in the same action map, OR by a sibling part of the same
        // composite (e.g. rebinding "Up" to the same key currently used by "Down").
        // Returns true + the name of whatever it collided with if a conflict is found.
        static bool CheckDuplicateBindings(InputAction actionToRebind, int bindingIndex, string newPath,
            bool allCompositeParts, out string conflictingActionName)
        {
            conflictingActionName = null;

            // Check against every OTHER action in the same action map (e.g. Move vs Jump).
            foreach (var binding in actionToRebind.actionMap.bindings)
            {
                if (binding.id == actionToRebind.bindings[bindingIndex].id)
                    continue; // skip comparing the binding against itself

                if (binding.effectivePath == newPath)
                {
                    var conflictingAction = actionToRebind.actionMap.FindAction(binding.action);
                    conflictingActionName = conflictingAction != null ? conflictingAction.name : binding.action;
                    return true;
                }
            }

            // Additionally check sibling parts of the SAME composite (Up/Down/Left/Right
            // live as separate bindings under one action, so the loop above alone won't
            // catch "Up" colliding with "Down" — both belong to the same action).
            if (allCompositeParts)
            {
                for (int i = 1; i < actionToRebind.bindings.Count; i++)
                {
                    if (i == bindingIndex) continue;
                    if (!actionToRebind.bindings[i].isPartOfComposite) continue;

                    if (actionToRebind.bindings[i].effectivePath == newPath)
                    {
                        conflictingActionName = actionToRebind.bindings[i].name; // e.g. "down"
                        return true;
                    }
                }
            }

            return false;
        }

        // Shows a temporary warning message, then hides it again after a delay.
        void ShowConflictMessage(string conflictingActionName)
        {
            if (conflictWarningText == null) return;

            conflictWarningText.text = $"That input is already used by \"{conflictingActionName}\"";
            conflictWarningText.gameObject.SetActive(true);

            CancelInvoke(nameof(HideConflictMessage)); // reset the timer if a new conflict happens quickly
            Invoke(nameof(HideConflictMessage), conflictWarningDuration);
        }

        void HideConflictMessage()
        {
            if (conflictWarningText != null)
                conflictWarningText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Optional: wire this to a "Reset to Default" button per row, or one button
        /// that resets all rows, to let the player undo their rebinds.
        /// </summary>
        public void ResetToDefault()
        {
            var resolvedAction = ResolveAction();
            if (resolvedAction == null) return;

            var bindingIndex = ResolveBindingIndex(resolvedAction);
            if (bindingIndex < 0) return;

            resolvedAction.RemoveBindingOverride(bindingIndex);
            UpdateBindingDisplay();
        }

        void OnDestroy()
        {
            // Safety net in case this row is destroyed mid-rebind (e.g. panel closed while waiting for input).
            rebindOperation?.Dispose();
        }
    }
}