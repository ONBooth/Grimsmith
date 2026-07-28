using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Games.UI
{
    /// <summary>
    /// Drop into your gameplay scene. Assign the "Pause" InputActionReference
    /// from your PlayerControls asset (Gameplay map).
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private InputActionReference pauseAction;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject tipsPanel;
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private bool _isPaused;

        private void OnEnable()
        {
            if (pauseAction == null) return;
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPauseTriggered;
        }

        private void OnDisable()
        {
            if (pauseAction == null) return;
            pauseAction.action.performed -= OnPauseTriggered;
        }

        private void OnPauseTriggered(InputAction.CallbackContext ctx)
        {
            // If settings are open, back out of them instead of unpausing.
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
                pausePanel.SetActive(true);
                return;
            }

            if (_isPaused) Resume();
            else Pause();
        }

        public void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }

        public void Resume()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }

        public void OnSettingsPressed()
        {
            pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        public void OnQuitToMenuPressed()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
        public void OnTipsPressed()
        {
            pausePanel.SetActive(false);
            tipsPanel.SetActive(true);
        }
    }
}