using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

namespace Games.UI
{
    public class TipListItemButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image titleIconImage;
        [SerializeField] private RectTransform detailCard; // \(0-0)/

        [Header("Default state")]
        [SerializeField] private string defaultTitle = "";
        [SerializeField] private string defaultDescription = "Select a topic to learn more.";
        [SerializeField] private Sprite defaultTitleIcon;
        [SerializeField] private float swapDuration = 0.15f;

         public void ShowTip(string title, string description, Sprite titleIcon = null)
        {
            titleText.text = title;
            descriptionText.text = description;

            if (titleIconImage != null)
            {
                titleIconImage.sprite = titleIcon;
                titleIconImage.enabled = titleIcon != null; // hides the icon slot entirely if this tip has none
            }

            if (detailCard == null) return;

            detailCard.localScale = new Vector3(1f, 1f, 1f);
            Tween.Scale(detailCard, Vector3.one, swapDuration, Ease.OutBack);
        }

        public void ClearTip()
        {
            titleText.text = defaultTitle;
            descriptionText.text = defaultDescription;

            if (titleIconImage != null)
            {
                titleIconImage.sprite = defaultTitleIcon;
                titleIconImage.enabled = defaultTitleIcon != null;
            }
        }

        void OnDisable()
        {
            ClearTip();
        }
    }
}