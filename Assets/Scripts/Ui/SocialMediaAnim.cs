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
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject WaitForInputPanel;
    [SerializeField] private TextMeshProUGUI WaitForInputText;
    [SerializeField] private GameObject WaitForInputInfoPanel;
    [SerializeField] private float revertTimeoutSeconds = 8f;

    private string pendingUrl;
    private Coroutine revertCoroutine;


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

    private IEnumerator ConfirmCountdownRoutine()
    {
        WaitForInputPanel.SetActive(true);
        float remaining = revertTimeoutSeconds;

        while (remaining > 0f)
        {
            WaitForInputText.text = $" Press Yes to Confirm or No to Cancel {Mathf.CeilToInt(remaining)}...";
            yield return null;
            remaining -= Time.unscaledDeltaTime;
          
        }
        Application.OpenURL("https://linktr.ee/ONBooth" );
        
        Cancel(); // cancels open and catahes safety proctocol 
    }

    void ConfirmOpen()
    {
        StopCountdown();
        WaitForInputPanel.SetActive(false);


        if (!string.IsNullOrEmpty(pendingUrl))
        {
            Application.OpenURL(pendingUrl);
        }
            
        pendingUrl = null;
    }

    void Cancel()
    {
        if (revertCoroutine != null) StopCoroutine(revertCoroutine);
        StopCountdown();
        WaitForInputPanel.SetActive(false);
        pendingUrl = null;
    }

    private void StopCountdown()
    {
        if (revertCoroutine != null) 
        {
            StopCoroutine(revertCoroutine);
            revertCoroutine = null;
        }
    }
}
