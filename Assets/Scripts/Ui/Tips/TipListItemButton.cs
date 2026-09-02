using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Games.UI
{
    public class TipListItemButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;

        int index;
        TipsMenuController controller;

        public RectTransform RectTransform => (RectTransform)transform;

        public void Setup(int index, string title, TipsMenuController controller)
        {
            this.index = index;
            this.controller = controller;
            label.text = title;

            button.onClick.AddListener(() => controller.SelectTip(index));
        }
    }
}