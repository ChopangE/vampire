using System;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;

namespace UI.Page
{
    [Binding]
    public class StartScenePage : PageViewModel
    {
        [Binding]
        public void PlayVideo()
        {
            var video = gameObject.GetComponentInChildren<Video>();
            video.Playing();
        }
        [Binding]
        public void OnClickStartButton()
        {
            Global.UIManager.OpenPage<CharacterSelectPage>();
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