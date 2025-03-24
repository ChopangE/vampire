using System;
using Data;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;

namespace UI.Page
{
    [Binding]
    public class IntroScenePage : PageViewModel
    {
        private IntroViewModel _introViewModel;
        private bool _isPlayed = false;
        protected override void Awake() {
            base.Awake();
            _introViewModel = GetComponentInChildren<IntroViewModel>();
        }
        private void OnEnable() {
            Global.SoundManager.PlayMusic(BGMEnum.TitleSpace);
        }
        [Binding]
        public void PlayVideo()
        {
            if(_isPlayed) return;
            _isPlayed = true;
            _introViewModel.PlayIntroCutScene();
        }
        [Binding]
        public void OnClickStartButton()
        {
            Global.UIManager.OpenPage<StartScenePage>();
        }
        [Binding]
        public void OnClickContinueButton()
        {
            //TODO 계속하기 버튼 구현
            //TODO UserDataManager에서 데이터 가져와서 이어하기
            OnClickStartButton();
            PlayVideo();
        }
        [Binding]
        public void OnClickOptionButton()
        {
            Global.UIManager.OpenPage<OptionPage>();
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