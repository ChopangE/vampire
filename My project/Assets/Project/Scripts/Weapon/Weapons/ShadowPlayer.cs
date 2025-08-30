using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using SO;
using Manager.InGame;

public class ShadowPlayer : Melee
{
    private GameObject shadowPlayer;
    private bool isShadowActive = false;

    private SpriteRenderer curPlayerSpriteRenderer;
    private SpriteRenderer shadowPlayerSpriteRenderer;
    
    protected override void Start()
    {
        InitializeComponents();
        curPlayerSpriteRenderer = GameManager.Instance.player.GetComponent<SpriteRenderer>();

        // Shadow_Partner 업적 클리어 (최초 획득 시)
        var userDataManager = Manager.Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (!killSaveData.ContainsKey("Shadow_Partner"))
            {
                killSaveData["Shadow_Partner"] = 1;
                userDataManager.Save();
                var achievementsManager = GameObject.FindObjectOfType<AchievementsManager>();
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID("Shadow_Partner");
                }
            }
        }
    }

    public override void LevelUp(UpgradeName prevUpgradeName, float prevUpgradeValue, DamageUpgradeValues damageUpgradeValues = null)
    {
        base.LevelUp(prevUpgradeName, prevUpgradeValue, damageUpgradeValues);
        // 레벨에 따른 보너스 데미지 업데이트
        GameManager.Instance.weaponController.bonusDamage = 0.2f + ((level - 1) * 0.1f);
    }
    
    public override void Attack()
    {
        base.Attack();
        if(playerSprite == null)
            return;
            
        // 플레이어 방향에 따른 스케일 조정
        if(playerSprite.flipX)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
        
        // 쉐도우가 활성화된 상태에서만 보너스 데미지 적용
        GameManager.Instance.weaponController.isBonusDamage = isShadowActive;
        
        // 쉐도우 플레이어가 존재할 때 스프라이트 업데이트
        if(shadowPlayer != null && isShadowActive)
        {
            shadowPlayerSpriteRenderer = shadowPlayer.GetComponent<SpriteRenderer>();
            if(shadowPlayerSpriteRenderer != null)
            {
                shadowPlayerSpriteRenderer.sprite = curPlayerSpriteRenderer.sprite;
                shadowPlayer.transform.localScale = Vector3.one;
            }
        }
    }
    
    public override async UniTask Init()
    {
        await base.Init();
    }

    public override void ExecuteAttack()
    {
        // 기존 검 소환 기능
        SetAttackDirection();
        shadowPlayer = SpawnSword();
        ConfigureSword(shadowPlayer, transform, true, new Vector3(-0.405f, 0, 0));
        
        // HGDClone의 총알 발사 기능 추가
        SpawnBullets().Forget();
        
        // 쉐도우 활성화 상태 관리
        StartShadowDuration().Forget();
    }
    
    public override async UniTask SpawnBullets()
    {
        for (int i = 0; i < count; i++)
        {
            SpawnBullet();
            
            if (i < count - 1)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(bulletSpawnInterval));
            }
        }
    }
    
    public override void SpawnBullet()
    {
        Vector3 dir = GetPlayerDirection();
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        bullet.GetComponent<Bullet>().Init(damage, pierce, dir, 
            criticalChancePercent: criticalChancePercent, 
            criticalDamagePercent: criticalDamagePercent);
    }
    
    private async UniTaskVoid StartShadowDuration()
    {
        isShadowActive = true;
        GameManager.Instance.weaponController.isBonusDamage = true;
        
        // duration 시간 동안 대기
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        
        // 쉐도우 비활성화
        isShadowActive = false;
        GameManager.Instance.weaponController.isBonusDamage = false;
    }
}