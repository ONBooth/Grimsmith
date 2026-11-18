using System;
using UnityEngine;
using UnityEngine.UI;

namespace  Games.UI
{
    public class CreditLinks : MonoBehaviour
    {
        [Serializable]
        public class Link
        {
            public string label; 
            public Sprite[] images;
            public string url;
        }
        [Header("References")]
        
        [SerializeField] private Transform namesContainer;

        [SerializeField] private SocialMediaAnim socialMedia; 
        [SerializeField] private Image Showimage;
        [SerializeField] private Button ShowNextImageButton;
        
        [Header("Data 0-10 = first person NameContainer")]
        [SerializeField] private Link[] people;
        
        private int currentPerson;
        private int currentImage;

        private void Awake()
        {
            // Wire a click listener onto every name entry, matching by child index.
            for (int i = 0; i < namesContainer.childCount && i < people.Length; i++)
            {
                int index = i; // copy, so each listener keeps its own index
                Transform child = namesContainer.GetChild(i);

                Button button = child.GetComponent<Button>();
                if (button == null) button = child.gameObject.AddComponent<Button>();

                button.onClick.AddListener(() => SelectPerson(index));
            }

            if (ShowNextImageButton != null)
                ShowNextImageButton.onClick.AddListener(NextImage);
        }

        private void OnDestroy()
        {
            // Child buttons are destroyed with the panel, so only the outside one needs removing.
            if (ShowNextImageButton != null)
                ShowNextImageButton.onClick.RemoveListener(NextImage);
        }

        private void SelectPerson(int index)
        {
            currentPerson = index;
            currentImage = 0;
            ShowCurrentImage();

            Debug.Log($"Clicked {index}, url = '{people[index].url}'");
            
            // Hand the link to your existing confirm / Yes-No / countdown flow.
            if (!string.IsNullOrEmpty(people[index].url))
                socialMedia.AskToOpen(people[index].url);
        }

        public void NextImage()
        {
            if (currentPerson < 0) return;
            var images = people[currentPerson].images;
            if (images == null || images.Length == 0) return;

            currentImage = (currentImage + 1) % images.Length; // wraps back to the first
            ShowCurrentImage();
        }

        private void ShowCurrentImage()
        {
            if (Showimage == null)
            {
                Debug.LogWarning("CreditLinks: Showimage is not assigned.");
                return;
            }

            var images = people[currentPerson].images;
            if (images == null || images.Length == 0)
            {
                Debug.LogWarning($"CreditLinks: person {currentPerson} has no images.");
                return;
            }

            Debug.Log($"Showing person {currentPerson}, image {currentImage} of {images.Length}");
            Showimage.sprite = images[currentImage];
        }

    }
}

