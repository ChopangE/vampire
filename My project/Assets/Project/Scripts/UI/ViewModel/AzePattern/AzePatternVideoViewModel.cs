using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityWeld;
using UnityWeld.Binding;
using UnityEngine.Video;

namespace UI
{
    [Binding]
    public class AzePatternVideoViewModel : ViewModel
    {
        [SerializeField] GameObject videoPlayer;
        // 비디오 완료 이벤트
        public event Action OnVideoCompleted;
        
        private bool isVideoPlaying = false;

        [Binding]
        public bool IsVideoPlaying
        {
            get => isVideoPlaying;
            set
            {
                isVideoPlaying = value;
                OnPropertyChanged(nameof(IsVideoPlaying));
            }
        }

        protected override void Awake()
        {
            base.Awake();
            IsVideoPlaying = false;
            videoPlayer.SetActive(false);
        }
        
        private void OnDestroy()
        {

        }
        
        // 비디오 재생
        public void PlayVideo()
        {
            IsVideoPlaying = true;
            videoPlayer.SetActive(true);
        }
        
        // 비디오 정지
        public void StopVideo()
        {
            IsVideoPlaying = false;
            videoPlayer.SetActive(false);
        }
        
        // 비디오 완료 처리
        public void OnVideoFinished()
        {
            OnVideoCompleted?.Invoke();
        }
    }
}
