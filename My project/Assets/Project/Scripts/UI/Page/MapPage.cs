using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;

namespace UI.Page
{
    [Binding]
    public class MapPage : PageViewModel
    {
        private int _coin;

        [Binding]
        public int Coin
        {
            get => _coin;
            set
            {
                _coin = value;
                OnPropertyChanged(nameof(Coin));
            }
        }
        [Binding]
        public void CallScene()
        {
            SceneManager.LoadScene("LoadingScene");
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