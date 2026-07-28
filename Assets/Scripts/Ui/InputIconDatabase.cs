using System;
using UnityEngine;

namespace Games.UI
{
    // ============================================================================
    // ICON DATABASE
    // A ScriptableObject asset that maps input control paths (e.g. "<Keyboard>/w")
    // to a Sprite icon. Create one via: Assets > Create > Input > Icon Database.
    // This lives as its own asset in your Project window, separate from any scene.
    // ============================================================================
    [CreateAssetMenu(fileName = "InputIconDatabase", menuName = "Input/Icon Database")]
    public class InputIconDatabase : ScriptableObject
    {
        [Serializable]
        public struct IconEntry
        {
            [Tooltip("Exact control path, e.g. <Keyboard>/w, <Gamepad>/buttonSouth, <Gamepad>/start")]
            public string controlPath;

            [Tooltip("Optional. Leave empty to match ANY device. Fill in to override for a specific " +
                     "controller layout — e.g. buttonSouth means 'A' on an Xbox pad but 'Cross' on a " +
                     "PlayStation pad, so you need separate entries per layout to show the right icon.")]
            public string deviceLayoutOverride;

            public Sprite icon;
        }

        [SerializeField] private IconEntry[] icons;

        /// <summary>
        /// Looks up the icon for a given control path. Tries an exact device-specific
        /// match first (e.g. PlayStation Cross vs Xbox A both use buttonSouth), then
        /// falls back to a generic entry with no device override, then null if nothing matches.
        /// </summary>
        public Sprite GetIcon(string controlPath, string deviceLayoutName)
        {
            foreach (var entry in icons)
            {
                if (entry.controlPath == controlPath &&
                    !string.IsNullOrEmpty(entry.deviceLayoutOverride) &&
                    entry.deviceLayoutOverride == deviceLayoutName)
                    return entry.icon;
            }

            foreach (var entry in icons)
            {
                if (entry.controlPath == controlPath && string.IsNullOrEmpty(entry.deviceLayoutOverride))
                    return entry.icon;
            }

            return null;
        }
    }
}
 

