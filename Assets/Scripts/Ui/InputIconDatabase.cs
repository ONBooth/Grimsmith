using System;
using System.Collections.Generic;
using UnityEngine;

namespace Games.UI
{
    /// <summary>
    /// Maps input control paths (e.g. "&lt;Keyboard&gt;/w", "&lt;XInputController&gt;/buttonSouth")
    /// to icon sprites, so RebindActionUI can show a glyph instead of plain text for
    /// the currently bound key/button.
    ///
    /// Create one via Assets → Create → Games → Input Icon Database, fill in entries
    /// for the keys/buttons your game actually uses, then drag it into RebindActionUI's
    /// "Icon Database" field. If you don't want icons at all, you can leave that field
    /// empty on every row — RebindActionUI already falls back to plain text automatically
    /// when no database is assigned or no matching icon is found.
    /// </summary>
    [CreateAssetMenu(fileName = "InputIconDatabase", menuName = "Games/Input Icon Database")]
    public class InputIconDatabase : ScriptableObject
    {
        [Serializable]
        public class IconEntry
        {
            [Tooltip("Exact control path this icon matches, e.g. <Keyboard>/w or <XInputController>/buttonSouth.")]
            public string controlPath;

            [Tooltip("Optional — restrict this entry to a specific device layout (e.g. \"XInputController\", " +
                      "\"DualShockGamepad\", \"Keyboard\"). Leave empty to match regardless of device.")]
            public string deviceLayoutName;

            public Sprite icon;
        }

        [SerializeField] private List<IconEntry> entries = new List<IconEntry>();

        /// <summary>
        /// Looks up the icon for a given control path, optionally narrowed by device layout.
        /// Returns null if no matching entry exists (caller should fall back to text).
        /// </summary>
        public Sprite GetIcon(string controlPath, string deviceLayoutName)
        {
            if (string.IsNullOrEmpty(controlPath))
                return null;

            // Prefer an exact match that also matches the device layout, if one was given.
            foreach (var entry in entries)
            {
                if (entry.controlPath == controlPath &&
                    !string.IsNullOrEmpty(entry.deviceLayoutName) &&
                    entry.deviceLayoutName == deviceLayoutName)
                    return entry.icon;
            }

            // Fall back to any entry matching just the control path, regardless of device.
            foreach (var entry in entries)
            {
                if (entry.controlPath == controlPath)
                    return entry.icon;
            }

            return null;
        }
    }
}