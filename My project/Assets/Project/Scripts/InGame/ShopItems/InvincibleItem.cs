using System.Collections;
using UnityEngine;
using Manager;

namespace InGame
{
    public class InvincibleItem : ActiveItemBase
    {
        [SerializeField] private float blinkInterval = 0.1f; // 깜빡임 간격

        private GameManager playerGameManager;

        public override void ActivateEffect()
        {
            // Debug.Log($"무적 효과 시작 (지속 시간: {duration}초)");

            // 플레이어 참조 가져오기
            playerGameManager = GameManager.Instance;

            if (playerGameManager != null)
            {
                // 플레이어 무적 상태 설정
                playerGameManager.isInvincible = true;

                // 깜빡임 효과 시작
                StartCoroutine(BlinkEffect());
            }
            else
            {
                Debug.LogWarning("플레이어 참조를 찾을 수 없습니다.");
                EndEffect();
            }
        }

        private IEnumerator BlinkEffect()
        {
            float endTime = Time.time + duration;

            while (Time.time < endTime)
            {
                // 투명도 조절로 깜빡임 효과
                yield return new WaitForSeconds(blinkInterval);
            }

            // 효과 종료
            EndEffect();
        }

        protected override void EndEffect()
        {
            Debug.Log("무적 효과 종료");

            // 플레이어 무적 상태 해제
            playerGameManager.isInvincible = false;

            // 코루틴 중지
            StopAllCoroutines();

            // 효과 종료 이벤트 발생 및 오브젝트 제거
            base.EndEffect();
        }

    }
}