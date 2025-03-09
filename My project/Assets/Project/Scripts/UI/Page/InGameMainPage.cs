using System;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;

namespace UI.Page
{
    [Binding]
    public class InGameMainPage : PageViewModel
    {        
        private bool _activeTimer;

        [Binding]
        public bool ActiveTimer
        {
            get => _activeTimer;
            set
            {
                _activeTimer = value;
                OnPropertyChanged(nameof(ActiveTimer));
            }
        }
        private string _gold;
        [Binding]
        public string Gold
        {
            get => _gold;
            set
            {
                _gold = value;
                OnPropertyChanged(nameof(Gold));
            }
        }

        private LevelUpPage levelUpPage;
        private OptionPage optionPage;
        private void OnEnable() {
            Global.GoldManager.OnGoldValueChanged += OnGoldValueChanged;
            Gold = Global.GoldManager.GetGoldText();
        }

        private void OnDisable() {
            Global.GoldManager.OnGoldValueChanged -= OnGoldValueChanged;
        }

        private void OnGoldValueChanged(object sender, string gold)
        {
            Gold = gold;
        }

        [Binding]
        public void Pause()
        {
            optionPage = Global.UIManager.OpenPage<OptionPage>();
        }
        [Binding]
        public void ShowLevelUP(bool isEvaluation = false)
        {
            levelUpPage = GetComponentInChildren<LevelUpPage>();
            levelUpPage.Show(isEvaluation);
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