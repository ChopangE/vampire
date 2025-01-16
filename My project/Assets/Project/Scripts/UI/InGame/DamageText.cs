using TMPro;
using UnityEngine;
using ObjectPooling;
using DG.Tweening;

namespace UI.InGame
{
    public class DamageText : PoolObject
    {
        [SerializeField] private TextMeshPro damageText;
        [SerializeField] private float moveDistance = 1f;
        [SerializeField] private float duration = 1f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private Color bonusColor = Color.blue;
        [SerializeField] private float criticalThreshold = 100f;
        [SerializeField] private float holdDuration = 0.3f;

        private void Awake()
        {
            if (damageText == null)
                damageText = GetComponent<TextMeshPro>();
        }

        public void SetDamage(float damage, bool isCritical = false, bool isBonus = false)
        {
            // 데미지 텍스트 설정
            damageText.text = damage.ToString("F0");
            damageText.color = isCritical || damage >= criticalThreshold ? criticalColor : isBonus ? bonusColor : normalColor;
            
            // 현재 위치를 기준으로 상대적으로 이동
            Vector3 startPos = transform.localPosition;
            transform.DOLocalMoveY(startPos.y + moveDistance, duration);
            
            // 텍스트를 잠시 유지한 후 페이드 아웃
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(holdDuration)  // 페이드 아웃 전 지연
                    .Append(damageText.DOFade(0f, duration))
                    .OnComplete(() => gameObject.SetActive(false));
        }

        protected override void OnDisable()
        {
            // 오브젝트가 비활성화될 때 모든 트윈 제거
            transform.DOKill();
            damageText.DOKill();
            base.OnDisable();
        }
    }
}