using UnityEngine;

namespace Games.UI
{
    [CreateAssetMenu(fileName = "TipEntry", menuName = "Tips/Tip Entry")]
    public class TipEntry : ScriptableObject
    {
        public string title;                 // e.g. "Help"

        [TextArea(2, 5)]
        [Tooltip("TMP rich text supported — use <color=#FFD700>Faith</color> etc. for highlighted keywords.")]
        public string description;

        public Sprite[] images;              // the paginated 1/3, 2/3, 3/3 gallery
    }
}