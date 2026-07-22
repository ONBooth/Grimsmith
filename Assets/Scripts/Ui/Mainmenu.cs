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

        private GameObject[] AllPanels => new[]
        {
            mainPanel, settingsPanel, audioPanel, graphicsPanel, controlsPanel, creditsPanel
        };

        private void Awake()
        {
            ShowOnly(mainPanel);
        }

        // ---- Central switcher: closes every panel, opens exactly one ----
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

        public void OnSettingsPressed() => ShowOnly(settingsPanel);
        public void OnSettingsClosed()  => ShowOnly(mainPanel);

        public void OnAudioPressed() => ShowOnly(audioPanel);
        public void OnAudioClosed()  => ShowOnly(settingsPanel);

        public void OnGraphicsPressed() => ShowOnly(graphicsPanel);
        public void OnGraphicsClosed()  => ShowOnly(settingsPanel);

        public void OnControlsPressed() => ShowOnly(controlsPanel);
        public void OnControlsClosed()  => ShowOnly(settingsPanel);

        public void OnCreditsPressed() => ShowOnly(creditsPanel);
        public void OnCreditsClosed()  => ShowOnly(settingsPanel);

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