using System;
using UnityEngine;

namespace InGame
{
    // 모든 활성 아이템이 구현해야 하는 인터페이스
    public interface IActiveItem
    {
        // 아이템 효과가 끝났을 때 발생하는 이벤트
        event Action OnEffectEnd;
        
        // 아이템이 플레이어에 부착되어야 하는지 여부
        bool AttachToPlayer { get; }
        
        // 아이템 효과 활성화
        void ActivateEffect();
    }
}