using System.Collections.Generic;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BossBullet : MonoBehaviour
{
    public float damage;
    public float duration;
    SpriteRenderer sprite;
    Collider2D coll;
    BossWeaponType type;
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
    }
    void Update()
    {
        if (!GameManager.Instance.isLive) return;
        if (type == BossWeaponType.Range)
        {
            transform.Translate(0, -3f * Time.deltaTime, 0);
        }
    }

    public void Init(float Damage, Vector3 dir, BossWeaponType bwt)
    {
        type = bwt;
        damage = Damage;
        switch (bwt)
        {
            case BossWeaponType.Bomb:
                PreStopAsync().Forget();
                StopAsync().Forget();
                break;
            case BossWeaponType.Range:
                BossBullet[] childBullet = GetComponentsInChildren<BossBullet>();
                foreach (BossBullet bb in childBullet)
                {
                    bb.damage = damage;
                }
                StopAsync().Forget();

                break;
        }
    }
    async UniTaskVoid PreStopAsync()
    {
        sprite.color = new Color(0.8f, 0, 0, 0.5f);
        coll.enabled = false;
        await UniTask.Delay(500); // 0.5초를 밀리초로 변환
        Global.SoundManager.PlaySFX(Data.SFXEnum.Witch_SpecialRemove);
        Global.SoundManager.PlaySFX(Data.SFXEnum.Witch_SpecialPunch);
        sprite.color = Color.white;
        coll.enabled = true;
    }
    async UniTaskVoid StopAsync()
    {
        await UniTask.Delay((int)(duration * 1000)); // 초를 밀리초로 변환
        gameObject.SetActive(false);
        Global.SoundManager.StopSFX(Data.SFXEnum.Witch_SpecialPunch);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        if (type == BossWeaponType.Range) // 까마귀 스킬
        {
            var damage = GameManager.Instance.maxHealth * 0.3f;
            GameManager.Instance.Health -= damage;
        }
        else if (type == BossWeaponType.Bomb) // 독병 직격
        {
            var damage = GameManager.Instance.maxHealth * 0.1f; // 최대 체력의 10%
            GameManager.Instance.Health -= damage;
        }
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        if (type == BossWeaponType.Bomb) // 독장판 효과
        {
            Player player = GameManager.Instance.player;
            
            // 2초간 30% 슬로우 적용
            if (!player.isSlowed)
            {
                player.ApplySlow(2f, 0.3f);
            }
            
            // 1초마다 최대 체력의 5% 중독 데미지 적용
            if (!player.isPoisoned)
            {
                player.ApplyPoison(1f, 0.05f);
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        if (type == BossWeaponType.Bomb) // 독장판에서 벗어날 때
        {
            Player player = GameManager.Instance.player;
            player.StopPoison(); // 중독 효과 중단
        }
    }
}
