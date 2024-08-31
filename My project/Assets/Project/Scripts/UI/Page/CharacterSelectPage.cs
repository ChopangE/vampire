using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;

namespace UI.Page
{
    [Binding]
    public class CharacterSelectPage : PageViewModel
    {
        [Binding]
        public void OnClickStartButton()
        {
            SceneManager.LoadScene("Map");
        }
        [Binding]
        public void OnClickOptionButton()
        {
        }
        [Binding]
        public void OnClickExitButton()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}