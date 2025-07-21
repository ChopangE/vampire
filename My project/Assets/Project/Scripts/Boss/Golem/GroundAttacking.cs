using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;

public class GroundAttacking : MonoBehaviour
{
    PolygonCollider2D coll;
    Golem golem;
    bool hasHitPlayer = false; // 중복 충돌 방지 플래그
    
    void Awake() {
        coll = GetComponent<PolygonCollider2D>();
        golem = GetComponentInParent<Golem>();
    }

    void OnEnable()
    {
        Global.SoundManager.PlaySFX(SFXEnum.Golem_HandDown);
        coll.enabled = false;
        hasHitPlayer = false; // 콜라이더 활성화 시 플래그 리셋
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer) return; // 이미 플레이어를 맞췄으면 무시
        
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            hasHitPlayer = true; // 플레이어를 맞췄음을 표시
            
            // 플레이어 최대 체력의 50% 데미지
            float damage = GameManager.Instance.maxHealth * 0.5f;
            GameManager.Instance.Health -= damage;
            
            // 2초간 30% 슬로우 효과
            player.ApplySlow(2f, 0.3f);
        }
    }
    
    public void ColliderOn() {
        coll.enabled = true;
    }
    public void EndAttack() {
        golem.EndDoing();
        gameObject.SetActive(false);
    }
}
