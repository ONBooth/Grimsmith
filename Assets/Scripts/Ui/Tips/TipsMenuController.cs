using UnityEngine;
using UnityEngine.UI;

namespace Games.UI
{ 
     [RequireComponent(typeof(Button))]
    public class TipsMenuController : MonoBehaviour
    {
        [Header("content for this button")]
        [SerializeField] private string tipTitle;
        [SerializeField] private Sprite tipTitleIcon; 

        [TextArea(2, 6)]
        [Tooltip("highlighted keywords.")]
        [SerializeField] private string tipDescription;

        [Header("Where to display it")]
        [SerializeField] private TipListItemButton tiplistitembutton;

        Button button;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(HandleClick);
        }

        void HandleClick()
        {
            tiplistitembutton.ShowTip(tipTitle, tipDescription,tipTitleIcon);
        }
    }
}
