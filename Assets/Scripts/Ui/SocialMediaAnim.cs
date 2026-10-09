using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class SocialMediaAnim : MonoBehaviour
{

    [Header("Buttons")]
    [SerializeField] private Button YesButton;
    [SerializeField] private Button NoButton;


    [Header("Confirm / Revert Safety Net")]
    [SerializeField] private GameObject WaitForInputPanel;
    [SerializeField] private TextMeshProUGUI WaitForInputText;
    [SerializeField] private float revertTimeoutSeconds = 8f;

    private string pendingUrl;
    Coroutine revertCoroutine;


    private void Awake()
    {
        YesButton.onClick.AddListener(ConfirmOpen);
        NoButton.onClick.AddListener(Cancel);
        WaitForInputPanel.SetActive(false);

    }

    private void OnDestroy()
    {
        YesButton.onClick.RemoveListener(ConfirmOpen);
        NoButton.onClick.RemoveListener(Cancel);
    }

   
    // passing the URL as the string argument.
    public void AskToOpen(string url)
    {
        pendingUrl = url;
        if (revertCoroutine != null) StopCoroutine(revertCoroutine);
        revertCoroutine = StartCoroutine(ConfirmCountdownRoutine());
    }

    IEnumerator ConfirmCountdownRoutine()
    {
        WaitForInputPanel.SetActive(true);
        float remaining = revertTimeoutSeconds;

        while (remaining > 0f)
        {
            WaitForInputText.text = $"Open link? Closing in {Mathf.CeilToInt(remaining)}...";
            yield return null;
            remaining -= Time.unscaledDeltaTime;
          
        }
        Application.OpenURL("https://onbooth.itch.io/" );
        Application.OpenURL("https://www.artstation.com/onbooth");
        Application.OpenURL("https://uk.linkedin.com/in/oneish-webb-nicholson-7627b11ba");
        Application.OpenURL("https://x.com/onb_ooth?s=11&t=oS4PE_6yfqfCCzOrEVsf1Q");
        Application.OpenURL("https://bsky.app/profile/onbooth.bsky.social");
        Cancel();
    }

    void ConfirmOpen()
    {
        if (revertCoroutine != null) StopCoroutine(revertCoroutine);
        WaitForInputPanel.SetActive(false);

        if (!string.IsNullOrEmpty(pendingUrl))
            

        
        pendingUrl = null;
    }

    void Cancel()
    {
        if (revertCoroutine != null) StopCoroutine(revertCoroutine);
        WaitForInputPanel.SetActive(false);
        pendingUrl = null;
    }
}
