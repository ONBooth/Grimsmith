using UnityEngine;
using UnityEngine.SceneManagement;

namespace Games.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "Gameplay";
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
       

        private void Awake()
        {
            // Force a known state on load, regardless of what was left
            // active in the editor.
            if (mainPanel != null) mainPanel.SetActive(true);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            
        }

        public void OnPlayPressed()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void OnSettingsPressed()
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        public void OnSettingsClosed()
        {
            settingsPanel.SetActive(false);
            mainPanel.SetActive(true);
        }
        
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