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

        // Animation state (Update-driven, allocation-free)
        private bool isAnimating = false;
        private float animStartTime = 0f;
        private Vector3 animStartPos;
        private Color animBaseColor;

        private void Awake()
        {
            if (damageText == null)
                damageText = GetComponent<TextMeshPro>();
        }

        public void SetDamage(float damage, bool isCritical = false, bool isBonus = false)
        {
            // 데미지 텍스트 설정 (avoid complex formatting allocations where possible)
            damageText.text = ((int)damage).ToString();

            // Determine base color and set alpha to fully visible
            animBaseColor = isCritical || damage >= criticalThreshold ? criticalColor : isBonus ? bonusColor : normalColor;
            animBaseColor.a = 1f;
            damageText.color = animBaseColor;

            // Start animation state
            animStartPos = transform.localPosition;
            animStartTime = Time.time;
            isAnimating = true;
        }

        void Update()
        {
            if (!isAnimating) return;

            float elapsed = Time.time - animStartTime;

            // movement: duration seconds from start position upward by moveDistance
            float moveT = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = animStartPos + Vector3.up * (moveDistance * moveT);

            // fade: start after holdDuration, then fade over duration seconds
            float fadeElapsed = Mathf.Clamp(elapsed - holdDuration, 0f, duration);
            float fadeT = Mathf.Clamp01(fadeElapsed / duration);
            Color c = animBaseColor;
            c.a = 1f - fadeT;
            damageText.color = c;

            // finish
            if (elapsed >= (holdDuration + duration))
            {
                isAnimating = false;
                gameObject.SetActive(false);
            }
        }

        protected override void OnDisable()
        {
            // Reset animation state so next activation starts clean
            isAnimating = false;
            base.OnDisable();
        }
    }
}