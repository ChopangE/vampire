using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;
namespace UI.Page
{
    [Binding]
    public class MasterBookPage : PageViewModel
    {
        private void OnEnable() {
            GameManager.Instance.Stop();
            Time.timeScale = 0.01f;
        }
        private void OnDisable() {
            GameManager.Instance.Resume();
        }
    }
}