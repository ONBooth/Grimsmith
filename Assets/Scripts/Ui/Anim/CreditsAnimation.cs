using System;
using UnityEngine;
using PrimeTween;

namespace Games.UI
{
    public class CreditsAnimation : MonoBehaviour
    {
        [Header("Container")]
        [SerializeField] private RectTransform creditsPanelRoot; // the CreditsPanel
        [SerializeField] private Transform namesContainer; // "Credits(Names)" — each child is one person/role entry
        

        [Header("Timing")]
        [SerializeField] private float stagger = 0.15f;
        [SerializeField] private float fadeDuration = 0.4f;
        [SerializeField] private float slideDistance = 25f;

        RectTransform[] entries;
        CanvasGroup[] entryGroups;
        Vector2[] restingPositions;
        Sequence activeSequence;

        void Awake()
        {
            CacheEntries();
            ResetEntries();
        }

        // Scans every child under namesContainer once, at startup — this is what lets
        // the script work with however many people/roles you have without a manual list.
        void CacheEntries()
        {
            int count = namesContainer.childCount;
            entries = new RectTransform[count];
            entryGroups = new CanvasGroup[count];
            restingPositions = new Vector2[count];

            for (int i = 0; i < count; i++)
            {
                var child = namesContainer.GetChild(i);
                var rect = child as RectTransform;
                entries[i] = rect;
                restingPositions[i] = rect.anchoredPosition;

                // CanvasGroup lets us fade the ENTIRE entry (name + role + any icons
                // inside it) with one alpha value, regardless of how many child
                // elements each entry actually contains.
                var group = child.GetComponent<CanvasGroup>();
                if (group == null) group = child.gameObject.AddComponent<CanvasGroup>();
                entryGroups[i] = group;
            }
        }

        // Puts every entry back to invisible + slightly below its resting spot,
        // so Play() always starts clean, however many times the credits are replayed.
        void ResetEntries()
        {
            for (int i = 0; i < entries.Length; i++)
            {
                entryGroups[i].alpha = 0f;
                entries[i].anchoredPosition = restingPositions[i] - new Vector2(0f, slideDistance);
            }
        }

        public void Play(Action onComplete = null)
        {
            activeSequence.Stop();
            creditsPanelRoot.gameObject.SetActive(true);
            ResetEntries();

            var seq = Sequence.Create();

            for (int i = 0; i < entries.Length; i++)
            {
                // Insert (not Chain) so each entry starts at its own exact time —
                // that's what creates the staggered, overlapping reveal rather than
                // waiting for each entry to fully finish before the next starts.
                float startTime = i * stagger;
                var group = entryGroups[i];
                var rect = entries[i];
                Vector2 target = restingPositions[i];

                seq.Insert(startTime, Tween.Custom(this, 0f, 1f, fadeDuration,
                    (t, alpha) => group.alpha = alpha, Ease.OutSine));
                seq.Insert(startTime, Tween.UIAnchoredPosition(rect, target, fadeDuration, Ease.OutCubic));
            }

            seq.ChainCallback(() => onComplete?.Invoke());
            activeSequence = seq;
        }
        public void Close(Action onComplete = null)
        {
            activeSequence.Stop();
            creditsPanelRoot.gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }
}  

    