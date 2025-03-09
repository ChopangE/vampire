using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;
namespace UI.Page
{
    [Binding]
    public class OptionPage : PageViewModel
    {
        private void OnEnable()
        {
            GameManager.Instance.Stop();
            Global.SoundManager.StopBGM(true);
        }
        private void OnDisable() {
            GameManager.Instance.Resume();
            Global.SoundManager.StopBGM(false);
        }
        [Binding]
        public void OnClickKorean()
        {
            I2.Loc.LocalizationManager.CurrentLanguage = "Korean";  //Language and Variant
            Debug.Log(I2.Loc.LocalizationManager.CurrentLanguage);
        }
        [Binding]
        public void OnClickEnglish()
        {
            I2.Loc.LocalizationManager.CurrentLanguage = "English (United States)";  //Language and Variant
            Global.UIManager.ClosePage();
        }
    }
}