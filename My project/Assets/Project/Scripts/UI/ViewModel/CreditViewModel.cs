using Data;
using Manager;
using UI.Page;
using UnityEngine;
using UnityEngine.Playables;
using UnityWeld;
using UnityWeld.Binding;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;

namespace UI
{
    [Binding]
    public class CreditViewModel : ViewModel
    {
        [SerializeField] 
        private RectTransform creditPanel;
        
        [SerializeField] 
        private float scrollSpeed = 50f; // 초당 스크롤 속도 (픽셀)
        
        [SerializeField] 
        private float totalScrollDistance = 1000f; // 총 스크롤 거리

        [SerializeField]
        private Vector2 scrollDirection = Vector2.up; // 스크롤 방향
        
        private CancellationTokenSource _scrollCts;
        
        private bool _isSkipButtonVisible = false;
        private bool _isScrolling = false;  // 스크롤 중인지 상태 추가
        
        [Binding]
        public bool IsSkipButtonVisible
        {
            get => _isSkipButtonVisible;
            set
            {
                _isSkipButtonVisible = value;
                OnPropertyChanged(nameof(IsSkipButtonVisible));
            }
        }

        private async void OnEnable()
        {
            try
            {
                _scrollCts = new CancellationTokenSource();
                IsSkipButtonVisible = true;
                _isScrolling = true;
                await ScrollCreditsAsync(_scrollCts.Token);
            }
            catch (OperationCanceledException)
            {
                // 취소는 정상적인 흐름이므로 무시
            }
            finally
            {
                if (_scrollCts != null)
                {
                    _scrollCts.Dispose();
                    _scrollCts = null;
                }
            }
        }

        private void OnDisable()
        {
            _scrollCts?.Cancel();
            _isScrolling = false;
            IsSkipButtonVisible = false;
        }

        [Binding]
        public void OnClickSkip()
        {
            if (!_isScrolling) return;
            
            _scrollCts?.Cancel();
            _isScrolling = false;
            IsSkipButtonVisible = false;
            
            OnCreditEnd();
        }

        private void OnCreditEnd()
        {
            // 여기에 크레딧이 끝났을 때의 처리를 추가
            // 예: 다른 씬으로 이동하거나 페이지를 닫는 등
            Global.UIManager.ClosePage();
            Global.UserDataManager.curStage = 0;
            SceneManager.LoadScene("StartScene");
        }

        private async UniTask ScrollCreditsAsync(CancellationToken cancellationToken)
        {
            try
            {
                Vector2 startPosition = creditPanel.anchoredPosition;
                float elapsedDistance = 0f;

                while (elapsedDistance < totalScrollDistance)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    float deltaMove = scrollSpeed * Time.deltaTime;
                    creditPanel.anchoredPosition += scrollDirection.normalized * deltaMove;
                    elapsedDistance += deltaMove;

                    await UniTask.Yield(cancellationToken);
                }

                // 자연스럽게 스크롤이 끝났을 때
                _isScrolling = false;
                IsSkipButtonVisible = false;
                OnCreditEnd();
            }
            catch (OperationCanceledException)
            {
                // 취소는 정상적인 흐름이므로 무시
                throw;
            }
        }
    }
}