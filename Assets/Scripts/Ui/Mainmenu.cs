using UnityEngine;
using UnityEngine.SceneManagement;

namespace Games.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "MainLevel"; 
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject audioPanel;
        [SerializeField] private GameObject graphicsPanel;
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject tipsPanel;

        [SerializeField] private SettingsPanelAnimator settingsAnimator;
        [SerializeField] private PausePanelAnimator panelAnimator;
        

        private GameObject[] AllPanels => new[]
        {
            mainPanel, settingsPanel, audioPanel, graphicsPanel, controlsPanel, creditsPanel, pausePanel , tipsPanel
        };

        private void Awake()
        {
            ShowOnly(mainPanel);
        }

       
        private void ShowOnly(GameObject panelToShow)
        {
            foreach (var panel in AllPanels)
            {
                if (panel == null) continue;
                panel.SetActive(panel == panelToShow);
            }
        }

        public void OnPlayPressed()
        {
            Debug.Log("OnPlayPressed called");
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void OnSettingsPressed()
        {
            Debug.Log("OnSettingsPressed called");
            ShowOnly(null); // hides everything else instantly
            settingsAnimator.Open();
        }

        public void OnSettingsClosed()
        {
            settingsAnimator.Close(); // deactivates itself when finished
            ShowOnly(mainPanel);
        }
        
        public void OnAudioPressed() => ShowOnly(audioPanel);
        public void OnAudioClosed()  => ShowOnly(settingsPanel);

        public void OnGraphicsPressed() => ShowOnly(graphicsPanel);
        public void OnGraphicsClosed()  => ShowOnly(settingsPanel);

        public void OnControlsPressed() => ShowOnly(controlsPanel);
        public void OnControlsClosed()  => ShowOnly(settingsPanel);

        public void OnCreditsPressed() => ShowOnly(creditsPanel);
        public void OnCreditsClosed()  => ShowOnly(settingsPanel);

        public void OnPausePressed()
        {
            panelAnimator.Open();
            ShowOnly(pausePanel);
        }

        public void OnPauseClosed()
        {
            panelAnimator.Close();
            ShowOnly(mainPanel);
        }
            
            
        
        public void onTipsPressed() => ShowOnly(tipsPanel);
        public void OnTipsClosed()  => ShowOnly(settingsPanel);
        

        public void OnQuitPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}