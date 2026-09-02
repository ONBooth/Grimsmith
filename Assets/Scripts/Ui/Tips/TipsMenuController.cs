using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

namespace Games.UI
{
    public class TipsMenuController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private TipEntry[] tips;

        [Header("List (right side, scrollable)")]
        [SerializeField] private RectTransform listContainer; // the ScrollView's "Content" object
        [SerializeField] private GameObject tipButtonPrefab;

        [Header("Detail Text Box (left side)")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private RectTransform detailCard; // for the punch-scale swap animation

        [Header("Optional Gallery (leave empty if not used)")]
        [SerializeField] private Image galleryImage;
        [SerializeField] private TextMeshProUGUI pageIndicatorText;
        [SerializeField] private Button nextImageButton;
        [SerializeField] private Button prevImageButton;

        [Header("Timing")]
        [SerializeField] private float cardSwapDuration = 0.15f;

        readonly List<TipListItemButton> spawnedButtons = new List<TipListItemButton>();
        int currentTipIndex = -1;
        int currentImageIndex;

        void Start()
        {
            BuildList();

            if (tips.Length > 0)
                SelectTip(0, animateCardSwap: false);

            if (nextImageButton != null) nextImageButton.onClick.AddListener(() => ChangeImage(1));
            if (prevImageButton != null) prevImageButton.onClick.AddListener(() => ChangeImage(-1));
        }

        void BuildList()
        {
            for (int i = 0; i < tips.Length; i++)
            {
                var go = Instantiate(tipButtonPrefab, listContainer);
                var listItem = go.GetComponent<TipListItemButton>();
                listItem.Setup(i, tips[i].title, this);
                spawnedButtons.Add(listItem);
            }
        }

        public void SelectTip(int index, bool animateCardSwap = true)
        {
            if (index == currentTipIndex || index < 0 || index >= tips.Length) return;

            currentTipIndex = index;
            currentImageIndex = 0;

            RefreshDetailCard(animateCardSwap);
        }

        void RefreshDetailCard(bool animateCardSwap)
        {
            var tip = tips[currentTipIndex];
            descriptionText.text = tip.description; // rich text tags render automatically via TMP

            if (galleryImage != null) UpdateGalleryImage();

            if (!animateCardSwap || detailCard == null) return;

            // Small punch-scale on the text box, so switching tips feels like a
            // deliberate swap rather than a flat instant text change.
            detailCard.localScale = new Vector3(0.96f, 0.96f, 1f);
            Tween.Scale(detailCard, Vector3.one, cardSwapDuration, Ease.OutBack);
        }

        void ChangeImage(int direction)
        {
            if (galleryImage == null) return;

            var tip = tips[currentTipIndex];
            if (tip.images == null || tip.images.Length == 0) return;

            currentImageIndex = (currentImageIndex + direction + tip.images.Length) % tip.images.Length;
            UpdateGalleryImage();
        }

        void UpdateGalleryImage()
        {
            var tip = tips[currentTipIndex];
            if (tip.images == null || tip.images.Length == 0)
            {
                galleryImage.enabled = false;
                if (pageIndicatorText != null) pageIndicatorText.text = string.Empty;
                return;
            }

            galleryImage.enabled = true;
            galleryImage.sprite = tip.images[currentImageIndex];
            if (pageIndicatorText != null)
                pageIndicatorText.text = $"{currentImageIndex + 1}/{tip.images.Length}";

            bool hasMultiple = tip.images.Length > 1;
            if (nextImageButton != null) nextImageButton.gameObject.SetActive(hasMultiple);
            if (prevImageButton != null) prevImageButton.gameObject.SetActive(hasMultiple);
        }
    }
}