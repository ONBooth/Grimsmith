using PrimeTween;
using TMPro;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class IDPanel : MonoBehaviour
{
    [SerializeField] private GameObject TopPanel;
    [SerializeField] private GameObject MiddlePanel;
    [SerializeField] private GameObject GeneralinforPanel;
    [SerializeField] private GameObject ButtomPanel;
    [SerializeField] private RectTransform Photo;
    [SerializeField] private Canvas canvas;
    [Space]
    [SerializeField] private GameObject Containter; // parent of the info texts
    [Space]
    [SerializeField] private float openTime = 0.4f;
    [SerializeField] private float slideTime = 0.45f;
    [SerializeField] private float photoStartX = 0f; // X where the photo sits in the middle

    private RectTransform top, middle, bottom;
    private CanvasGroup photoGroup, infoGroup;
    private Vector2 topOpen, bottomOpen, topClosed, bottomClosed, photoLeft, photoCentre;
    private Sequence sequence;
    private TextMeshProUGUI[] texts;

    private void Awake()
    {
        top = (RectTransform)TopPanel.transform;
        middle = (RectTransform)MiddlePanel.transform;
        bottom = (RectTransform)ButtomPanel.transform;
        photoGroup = GetGroup(Photo.gameObject);
        infoGroup = GetGroup(GeneralinforPanel);
        texts = Containter.GetComponentsInChildren<TextMeshProUGUI>(true); // every text under the parent

        // The layout you built in the editor is the OPEN layout
        topOpen = top.anchoredPosition;
        bottomOpen = bottom.anchoredPosition;
        photoLeft = Photo.anchoredPosition;

        // Closed: top and bottom meet in the middle
        Vector2 half = new Vector2(0f, middle.rect.height * 0.5f);
        topClosed = topOpen - half;
        bottomClosed = bottomOpen + half;
        photoCentre = new Vector2(photoStartX, photoLeft.y);

        SnapClosed();
    }

    // Checks every frame for the P key
    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        bool pressed = Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame;
#else
        bool pressed = Input.GetKeyDown(KeyCode.P);
#endif
        if (pressed) Show();
    }

    // Called by the P key (a button can call it too)
    public void Show()
    {
        canvas.enabled = true;   // make the canvas visible
        SnapClosed();            // reset so it replays every time
        Open();
    }

    public void Hide()
    {
        sequence.Stop();
        canvas.enabled = false;
    }

    // Instantly puts everything back to closed
    private void SnapClosed()
    {
        sequence.Stop();
        top.anchoredPosition = topClosed;
        bottom.anchoredPosition = bottomClosed;
        middle.localScale = new Vector3(1f, 0f, 1f);
        Photo.anchoredPosition = photoCentre;
        Photo.localRotation = Quaternion.identity;
        photoGroup.alpha = 0f;
        infoGroup.alpha = 0f;
    }

    [ContextMenu("Open")]
    public void Open()
    {
        StopAnimation();
        // Hide all the letters before the animation starts
        foreach (TextMeshProUGUI text in texts) text.maxVisibleCharacters = 0;
        sequence = Sequence.Create(useUnscaledTime: true)
            // 1. The case file splits open
            .Group(Tween.UIAnchoredPosition(top, topOpen, openTime, Ease.OutBack))
            .Group(Tween.UIAnchoredPosition(bottom, bottomOpen, openTime, Ease.OutBack))
            .Group(Tween.ScaleY(middle, 1f, openTime, Ease.OutCubic))
            // 2. The photo appears in the middle
            .Chain(Tween.Alpha(photoGroup, 1f, 0.15f))
            .ChainDelay(0.15f)
            // 3. It slides left and wobbles, like a photo tossed on a desk
            .Chain(Tween.UIAnchoredPosition(Photo, photoLeft, slideTime, Ease.InOutCubic))
            .Chain(Tween.PunchLocalRotation(Photo, new Vector3(0f, 0f, 8f), 0.35f, frequency: 7))
            // 4. The info is "typed up" like a detective's report
            .Group(Tween.Alpha(infoGroup, 1f, 0.2f));

        // Type each text one after another, in Hierarchy order
        foreach (TextMeshProUGUI text in texts)
        {
            text.ForceMeshUpdate(true);
            int letters = text.textInfo.characterCount;
            if (letters == 0) continue;

            sequence.Chain(Tween.Custom(text, 0f, letters, letters * 0.025f,
                (target, value) => target.maxVisibleCharacters = Mathf.CeilToInt(value), Ease.Linear));
        }
    }

    [ContextMenu("Close")]
    public void Close()
    {
        StopAnimation();

        sequence = Sequence.Create(useUnscaledTime: true)
            .Group(Tween.Alpha(infoGroup, 0f, 0.15f))
            .Group(Tween.Alpha(photoGroup, 0f, 0.15f))
            .Chain(Tween.UIAnchoredPosition(top, topClosed, openTime, Ease.InBack))
            .Group(Tween.UIAnchoredPosition(bottom, bottomClosed, openTime, Ease.InBack))
            .Group(Tween.ScaleY(middle, 0f, openTime, Ease.InCubic))
            .ChainCallback(this, panel => panel.Photo.anchoredPosition = panel.photoCentre);
    }

    // Stops the current animation so clicking quickly can't make two fight each other
    private void StopAnimation()
    {
        sequence.Stop();
        Photo.localRotation = Quaternion.identity;
    }

    private void OnDisable() => sequence.Stop();

    private static CanvasGroup GetGroup(GameObject target)
    {
        return target.TryGetComponent(out CanvasGroup group) ? group : target.AddComponent<CanvasGroup>();
    }
}