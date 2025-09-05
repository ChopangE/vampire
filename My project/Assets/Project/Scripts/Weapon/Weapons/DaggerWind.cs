using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using SO;
using Manager.InGame;
using Data;
using Manager;
public class DaggerWind : Melee
{
    public int windCount = 1;  // 기본 검풍 개수
    public float windDelay = 0.1f;  // 검풍 사이 지연시간


    public override async UniTask Init()
    {
        await base.Init();

        // Sword_wind 업적 클리어 (최초 획득 시)
        var userDataManager = Manager.Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (!killSaveData.ContainsKey("Sword_wind"))
            {
                killSaveData["Sword_wind"] = 1;
                userDataManager.Save();
                var achievementsManager = GameObject.FindObjectOfType<AchievementsManager>();
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID("Sword_wind");
                }
            }
        }
    }

    public override void SpawnBullet()
    {
        SetAttackDirection();
        GameObject sword = SpawnSword();
        ConfigureSword(sword, null, true);
        SpawnWindSlashes().Forget();
    }

    private async UniTaskVoid SpawnWindSlashes()
    {
        for (int i = 0; i < windCount; i++)
        {
            Global.SoundManager.PlaySFX(SFXEnum.SwordWind);
            Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            
            float angle = dir.x < 0 ? 0f : 180f;
            bullet.rotation = Quaternion.Euler(0, 0, angle);
            
            // 인덱스가 증가할수록 크기가 작아지도록 설정
            float scaleFactor = 1f - (i * 0.2f); // 각 검풍마다 20%씩 작아짐
            scaleFactor = Mathf.Max(scaleFactor, 0.4f); // 최소 크기는 원본의 40%로 제한
            bullet.localScale = Vector3.one * scaleFactor;
            
            var bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.Init(damage * 0.5f, -1, dir, duration: duration, criticalChancePercent: criticalChancePercent, criticalDamagePercent: criticalDamagePercent);
            bulletComponent.rb.velocity = dir * 14f;

            if (i < windCount - 1)
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(windDelay));
            }
        }
    }

    public override void LevelUp(UpgradeName prevUpgradeName, float prevUpgradeValue, DamageUpgradeValues damageUpgradeValues = null)
    {
        base.LevelUp(prevUpgradeName, prevUpgradeValue, damageUpgradeValues);
        windCount = count + 1;
    }


}