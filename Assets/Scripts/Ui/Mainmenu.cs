using UnityEngine;
using UnityEngine.SceneManagement;

namespace Games.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "Gameplay";
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject audioPanel;
        [SerializeField] private GameObject graphicsPanel;
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private GameObject PausePanel;

        [SerializeField] private SettingsPanelAnimator settingsAnimator;
        

        private GameObject[] AllPanels => new[]
        {
            mainPanel, settingsPanel, audioPanel, graphicsPanel, controlsPanel, creditsPanel, PausePanel
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
        
        public void OnPausePressed() => ShowOnly(PausePanel);
        public void OnPauseClosed() => ShowOnly(mainPanel);
        

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