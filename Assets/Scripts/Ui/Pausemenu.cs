using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Games.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private InputActionReference pauseAction;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject tipsPanel;
        [SerializeField] private string mainMenuSceneName = "Grimsmith";

        [SerializeField] private PausePanelAnimator panelAnimator; 
        

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
            panelAnimator.Open(); // handles Time.timeScale = 0f AND SetActive(true) internally, plus the animation
        }

        public void Resume()
        {
            _isPaused = false;
            panelAnimator.Close(); // handles Time.timeScale = 1f AND SetActive(false) internally, plus the animation
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

        public void TogglePause()
        {
            if (_isPaused) Resume();
            else Pause();

        }
        
    }
}