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

        private LevelUpPage levelUpPage;
        private PausePage pausePage;

        [Binding]
        public void Pause()
        {
            pausePage = GetComponentInChildren<PausePage>();
            pausePage.Show();
        }
        [Binding]
        public void ShowLevelUP()
        {
            levelUpPage = GetComponentInChildren<LevelUpPage>();
            levelUpPage.Show();
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