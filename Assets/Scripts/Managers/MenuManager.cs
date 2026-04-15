using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MenuManager : MonoBehaviour
    {
        private void Start()
        {
            MusicManager.Instance.PlayMainMenuMusic();
        }
        
        public void StartGame()
        {
            SceneManager.LoadScene("Scenes/Inicio");
        }
        
        public void QuitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #endif
        }
    }
}