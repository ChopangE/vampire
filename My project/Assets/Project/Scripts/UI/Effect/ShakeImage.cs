using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

// DOTween과 UniTask를 연결하는 확장 메서드
public static class DOTweenUniTaskExtension
{
    public static async UniTask ToUniTask(this Tween tween, CancellationToken cancellationToken = default)
    {
        if (tween == null || !tween.IsActive() || cancellationToken.IsCancellationRequested)
        {
            tween?.Kill();
            return;
        }

        var cancellationTokenRegistration = cancellationToken.Register(() => 
        {
            if (tween != null && tween.IsActive())
            {
                tween.Kill();
            }
        });

        try
        {
            await tween.AsyncWaitForCompletion();
        }
        catch (Exception)
        {
            // DOTween 예외 처리
            if (tween != null && tween.IsActive())
            {
                tween.Kill();
            }
            throw;
        }
        finally
        {
            cancellationTokenRegistration.Dispose();
        }
    }
}

public class ShakeImage : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 10f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private float shakeRandomness = 90f;
    [SerializeField] private bool fadeOut = true;
    
    private Image targetImage;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private CancellationTokenSource shakeCts;
    private bool isDestroyed = false;
    
    private void Awake()
    {
        targetImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition;
    }
    
    /// <summary>
    /// 이미지를 흔드는 효과를 시작합니다.
    /// </summary>
    public void Shake()
    {
        // 파괴 시 자동으로 취소되는 토큰 사용
        ShakeAsync(null, this.GetCancellationTokenOnDestroy()).Forget();
    }
    
    /// <summary>
    /// 지정된 지속 시간으로 이미지를 흔드는 효과를 시작합니다.
    /// </summary>
    /// <param name="duration">흔들림 지속 시간</param>
    public void Shake(float duration)
    {
        // 파괴 시 자동으로 취소되는 토큰 사용
        ShakeAsync(duration, this.GetCancellationTokenOnDestroy()).Forget();
    }
    
    /// <summary>
    /// 이미지를 흔드는 비동기 메서드
    /// </summary>
    /// <param name="duration">흔들림 지속 시간 (기본값 사용시 null)</param>
    /// <returns>UniTask</returns>
    public async UniTask ShakeAsync(float? duration = null, CancellationToken cancellationToken = default)
    {
        // 이미 파괴되었거나 취소 요청된 경우 실행하지 않음
        if (isDestroyed || cancellationToken.IsCancellationRequested || this == null)
        {
            return;
        }
        
        // 이전 흔들림 효과가 있다면 중지
        StopShake();
        
        // 새로운 CancellationTokenSource 생성
        shakeCts = new CancellationTokenSource();
        var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(shakeCts.Token, cancellationToken);
        
        try
        {
            // 컴포넌트가 유효한지 확인
            if (rectTransform == null || !this || isDestroyed)
            {
                return;
            }
            
            // 원래 위치로 리셋
            rectTransform.localPosition = originalPosition;
            
            // 흔들림 효과 생성 및 실행
            float shakeDurationValue = duration ?? shakeDuration;
            
            // DOTween 애니메이션 생성 및 UniTask로 변환하여 실행
            var tween = rectTransform.DOShakePosition(
                shakeDurationValue, 
                shakeStrength, 
                shakeVibrato, 
                shakeRandomness, 
                false, 
                fadeOut
            ).SetUpdate(true);
            
            await tween.ToUniTask(combinedCts.Token);
            
            // 효과 종료 후 원래 위치로 복귀 (컴포넌트가 유효한 경우에만)
            if (rectTransform != null && this != null && !isDestroyed)
            {
                rectTransform.localPosition = originalPosition;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소된 경우 원래 위치로 복귀 (컴포넌트가 유효한 경우에만)
            if (rectTransform != null && this != null && !isDestroyed)
            {
                rectTransform.localPosition = originalPosition;
            }
        }
        catch (Exception)
        {
            // 다른 예외 발생 시 처리 (컴포넌트가 유효한 경우에만)
            if (rectTransform != null && this != null && !isDestroyed)
            {
                rectTransform.localPosition = originalPosition;
            }
        }
        finally
        {
            if (combinedCts != null)
            {
                combinedCts.Dispose();
            }
            shakeCts = null;
        }
    }
    
    /// <summary>
    /// 현재 진행 중인 흔들림 효과를 중지합니다.
    /// </summary>
    public void StopShake()
    {
        if (shakeCts != null && !shakeCts.IsCancellationRequested)
        {
            shakeCts.Cancel();
            shakeCts.Dispose();
            shakeCts = null;
        }
        
        // DOTween 애니메이션 중지 (컴포넌트가 유효한 경우에만)
        if (rectTransform != null && this != null && !isDestroyed)
        {
            rectTransform.DOKill();
            rectTransform.localPosition = originalPosition;
        }
    }
    
    private void OnDestroy()
    {
        isDestroyed = true;
        StopShake();
    }
    
    private void OnDisable()
    {
        StopShake();
    }
}
