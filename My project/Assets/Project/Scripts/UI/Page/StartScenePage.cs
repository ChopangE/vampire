using System;
using Data;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;
using System.Collections;

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
            Global.SoundManager.PlaySFX(SFXEnum.OpenButton);
            StartCoroutine(LoadSceneAfterSound(SFXEnum.OpenButton));
        }

        private IEnumerator LoadSceneAfterSound(SFXEnum soundEnum)
        {
            // 효과음 클립의 길이를 가져옴
            float soundDuration = Global.SoundManager.GetSFXClipLength(soundEnum);
            
            // 효과음이 재생될 동안 대기
            yield return new WaitForSeconds(soundDuration);
            
            // 효과음 재생 완료 후 씬 로드
            SceneManager.LoadScene("Map");
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