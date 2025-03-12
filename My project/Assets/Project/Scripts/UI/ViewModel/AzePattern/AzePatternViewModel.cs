using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manager;
using UnityEngine;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class AzePatternViewModel : GroupView
    {
        [SerializeField] private AzePatternVideoViewModel azePatternVideoViewModel;
        private const int ARROW_COUNT = 10;
        private const float TIME_LIMIT = 5f;
        private const float CLOSE_DELAY = 3f; // 성공 후 페이지 닫기까지의 지연 시간
        
        private List<Vector2> arrowDirections = new List<Vector2>();
        private int currentArrowIndex = 0;
        private bool isPatternActive = false;
        private float remainingTime = 0f;
        private System.Threading.CancellationTokenSource patternTimerCts;
        
        // 슬라이더용 남은 시간 비율 (0.0 ~ 1.0)
        private float timeRatio = 1.0f;
        
        // 텍스트 표현용 남은 시간 문자열
        private string remainingTimeText = "";
        
        // 패턴 성공/실패 이벤트 추가
        public event Action OnPatternSucceeded;
        public event Action OnVideoEnded;
        public event Action OnPatternFailed;
        
        [Binding]
        public bool IsPatternActive
        {
            get => isPatternActive;
            set
            {
                isPatternActive = value;
                OnPropertyChanged(nameof(IsPatternActive));
            }
        }
        
        [Binding]
        public float RemainingTime
        {
            get => remainingTime;
            set
            {
                remainingTime = value;
                // 남은 시간 텍스트도 함께 업데이트
                RemainingTimeText = $"{remainingTime:F1}s";
                OnPropertyChanged(nameof(RemainingTime));
            }
        }
        
        // 텍스트 표현용 남은 시간 (바인딩용)
        [Binding]
        public string RemainingTimeText
        {
            get => remainingTimeText;
            set
            {
                remainingTimeText = value;
                OnPropertyChanged(nameof(RemainingTimeText));
            }
        }
        
        [Binding]
        public float TimeRatio
        {
            get => timeRatio;
            set
            {
                timeRatio = value;
                OnPropertyChanged(nameof(TimeRatio));
            }
        }
        
        private void OnEnable()
        {
            // 화살표 10개 생성
            GenerateArrowPattern();
            
            // 타이머 시작
            StartPatternTimer();
            
            // 입력 이벤트 구독
            SubscribeToInputEvents();
        }
        
        private void OnDisable()
        {
            // 입력 이벤트 구독 해제
            UnsubscribeFromInputEvents();
            
            // 타이머 중지
            CancelPatternTimer();
            
            IsPatternActive = false;
        }
        
        private void GenerateArrowPattern()
        {
            // 화살표 방향 리스트 초기화
            arrowDirections.Clear();
            
            // 10개의 랜덤 방향 생성
            for (int i = 0; i < ARROW_COUNT; i++)
            {
                int randomDirection = UnityEngine.Random.Range(0, 4);
                Vector2 direction = Vector2.zero;
                
                switch (randomDirection)
                {
                    case 0: // 위
                        direction = Vector2.up;
                        break;
                    case 1: // 오른쪽
                        direction = Vector2.right;
                        break;
                    case 2: // 아래
                        direction = Vector2.down;
                        break;
                    case 3: // 왼쪽
                        direction = Vector2.left;
                        break;
                }
                
                arrowDirections.Add(direction);
            }
            
            IsPatternActive = true;
            // 화살표 뷰모델 생성
            PrepareViewModels(ARROW_COUNT);
            var models = GetViewModels();
            
            // 각 화살표에 방향 설정
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i] is AzePatternArrowViewModel arrowViewModel)
                {
                    arrowViewModel.SetDirection(arrowDirections[i]);
                    arrowViewModel.SetActive(true);
                }
            }
            
            // 패턴 활성화
            currentArrowIndex = 0;
        }
        
        private void StartPatternTimer()
        {
            // 이전 타이머가 있다면 취소
            if (patternTimerCts != null)
            {
                patternTimerCts.Cancel();
                patternTimerCts.Dispose();
            }
            
            // 새 취소 토큰 생성
            patternTimerCts = new System.Threading.CancellationTokenSource();
            
            // UniTask로 타이머 실행
            PatternTimerAsync(patternTimerCts.Token).Forget();
        }
        
        private async UniTaskVoid PatternTimerAsync(System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                // 시작 시간 기록 (실제 시간 기준)
                float startTime = Time.realtimeSinceStartup;
                float endTime = startTime + TIME_LIMIT;
                
                while (IsPatternActive)
                {
                    // 현재 실제 시간
                    float currentTime = Time.realtimeSinceStartup;
                    
                    // 남은 시간 계산 (TimeScale에 영향 받지 않음)
                    RemainingTime = Mathf.Max(0, endTime - currentTime);
                    
                    // 슬라이더용 시간 비율 업데이트 (0.0 ~ 1.0)
                    TimeRatio = Mathf.Clamp01(RemainingTime / TIME_LIMIT);
                    
                    // 시간 초과 시 패턴 실패
                    if (RemainingTime <= 0)
                    {
                        PatternFailed();
                        break;
                    }
                    
                    // 실제 시간 기준으로 다음 프레임까지 대기 (TimeScale 영향 없음)
                    await UniTask.DelayFrame(1, PlayerLoopTiming.Update, cancellationToken: cancellationToken);
                }
            }
            catch (System.OperationCanceledException)
            {
                // 타이머가 취소된 경우 (정상적인 상황)
                Debug.Log("패턴 타이머가 취소되었습니다.");
            }
            catch (System.Exception ex)
            {
                // 기타 예외 처리
                Debug.LogError($"패턴 타이머 오류: {ex.Message}");
            }
        }
        
        private void SubscribeToInputEvents()
        {
            if (Global.InputManager != null)
            {
                Global.InputManager.OnArrowKeyAction += HandleArrowKeyInput;
            }
        }
        
        private void UnsubscribeFromInputEvents()
        {
            if (Global.InputManager != null)
            {
                Global.InputManager.OnArrowKeyAction -= HandleArrowKeyInput;
            }
        }
        
        private void HandleArrowKeyInput(object sender, Vector2 direction)
        {
            if (!IsPatternActive || currentArrowIndex >= ARROW_COUNT)
                return;
            
            // 현재 화살표와 입력 방향 비교
            if (direction == arrowDirections[currentArrowIndex])
            {
                // 올바른 입력
                var models = GetViewModels();
                if (currentArrowIndex < models.Count && models[currentArrowIndex] is AzePatternArrowViewModel arrowViewModel)
                {
                    // 화살표 상태 변경 (회색으로)
                    arrowViewModel.SetCompleted(true);
                }
                
                // 다음 화살표로 이동
                currentArrowIndex++;
                
                // 모든 화살표 입력 완료 시 성공
                if (currentArrowIndex >= ARROW_COUNT)
                {
                    PatternSucceeded().Forget();
                }
            }
            else
            {
                // 잘못된 입력 - 패턴 실패
                PatternFailed();
            }
        }
        
        private async UniTask PatternSucceeded()
        {
            // Debug.Log("아재 패턴 성공!");
            IsPatternActive = false;
            CancelPatternTimer();
            
            // 패턴 성공 이벤트 발생
            OnPatternSucceeded?.Invoke();
            
            Time.timeScale = 1f;
            // 비디오 재생 (기존 코드)
            azePatternVideoViewModel.PlayVideo();
            
            // UniTask를 사용하여 5초 후 페이지 닫기
            await UniTask.Delay(TimeSpan.FromSeconds(CLOSE_DELAY), cancellationToken: this.GetCancellationTokenOnDestroy());
            
            // 페이지가 아직 활성화 상태인지 확인 후 닫기
            if (this != null && this.gameObject != null && this.gameObject.activeInHierarchy)
            {
                // Debug.Log("아재 패턴 성공 후 5초 지연 - 페이지 닫기");
                azePatternVideoViewModel.StopVideo();
                azePatternVideoViewModel.OnVideoFinished();
                Global.UIManager.ClosePage();
            }
        }
        
        private void PatternFailed()
        {
            // Debug.Log("아재 패턴 실패!");
            IsPatternActive = false;
            CancelPatternTimer();
            
            // 패턴 실패 이벤트 발생
            OnPatternFailed?.Invoke();
            
            // 페이지 닫기
            Global.UIManager.ClosePage();
        }
        
        private void CancelPatternTimer()
        {
            if (patternTimerCts != null)
            {
                patternTimerCts.Cancel();
                patternTimerCts.Dispose();
                patternTimerCts = null;
            }
        }
    }
}
