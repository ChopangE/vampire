using System;
using UnityEngine;

namespace InGame
{
    // 활성 아이템의 기본 구현
    public abstract class ActiveItemBase : MonoBehaviour, IActiveItem
    {
        [SerializeField] protected float duration = 10f;
        [SerializeField] protected bool attachToPlayer = true;
        
        public event Action OnEffectEnd;
        
        public bool AttachToPlayer => attachToPlayer;
        
        protected virtual void Start()
        {
            ActivateEffect();
        }
        
        public virtual void ActivateEffect()
        {
            // 지속 시간 후 효과 종료
            Invoke("EndEffect", duration);
        }
        
        protected virtual void EndEffect()
        {
            // 효과 종료 이벤트 발생
            OnEffectEnd?.Invoke();
            
            // 오브젝트 제거
            Destroy(gameObject);
        }
    }
}