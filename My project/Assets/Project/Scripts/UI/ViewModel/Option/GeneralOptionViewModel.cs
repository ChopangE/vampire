using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityWeld;
using UnityWeld.Binding;


namespace UI
{
    [Binding]
    public class GeneralOptionViewModel : ViewModel
    {
        private string _language;

        [Binding]
        public string Language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged(nameof(Language));
            }
        }
        private void OnEnable() {
            // 현재 언어가 한국어인지 확인하고 토글
            if (I2.Loc.LocalizationManager.CurrentLanguage == "Korean")
                Language = "한국어";
            else
                Language = "English";
        }

        [Binding]
        public void OnClickLanguage()
        {
            // 현재 언어가 한국어인지 확인하고 토글
            if (I2.Loc.LocalizationManager.CurrentLanguage == "Korean")
            {
                I2.Loc.LocalizationManager.CurrentLanguage = "English (United States)";
                Language = "English";
            }
            else
            {
                I2.Loc.LocalizationManager.CurrentLanguage = "Korean";
                Language = "한국어";
            }
        }


        [Binding]
        public void OnClickRetry()
        {
            Global.UIManager.ClosePage();
            // 다시 시도 기능 구현
            if(Global.UserDataManager.curStage == 0) {
                Global.DataManager.ResetData();
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
            }
            else {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
            }
        }

        [Binding]
        public void OnClickExit()
        {
            Global.UIManager.ClosePage();
            // 게임 종료 또는 메인 메뉴로 돌아가기
            UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
        }

        [Binding]
        public void SaveSettings()
        {
            // 설정 저장 기능
            PlayerPrefs.SetString("Language", I2.Loc.LocalizationManager.CurrentLanguage);
            PlayerPrefs.Save();
        }
    }
}
