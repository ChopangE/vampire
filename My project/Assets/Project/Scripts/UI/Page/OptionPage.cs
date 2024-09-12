using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;
namespace UI.Page
{
    [Binding]
    public class OptionPage : PageViewModel
    {
        [Binding]
        public void OnClickKorean()
        {
            I2.Loc.LocalizationManager.CurrentLanguage = "English (United States)";  //Language and Variant
        }
        [Binding]
        public void OnClickEnglish()
        {
            I2.Loc.LocalizationManager.CurrentLanguage = "English (United States)";  //Language and Variant
            Global.UIManager.ClosePage();
        }
    }
}