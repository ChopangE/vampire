using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using SO;
using Manager.InGame;
using Data;
using Manager;

public class Bowling : BulletWeapon
{
    public override async UniTask Init()
    {
        await base.Init();
        maxCooldown = 5f;
        remainingCooldown = maxCooldown;

        // Catapul 업적 클리어 (최초 획득 시)
        var userDataManager = Manager.Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (!killSaveData.ContainsKey("Catapul"))
            {
                killSaveData["Catapul"] = 1;
                userDataManager.Save();
                var achievementsManager = GameObject.FindObjectOfType<AchievementsManager>();
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID("Catapul");
                }
            }
        }
    }
    public override void SpawnBullet()
    {
        if (!player.scan.nearestTarget) return;
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;

        bullet.position = transform.position;
        var bulletScript = bullet.GetComponent<BowlingBullet>();
        bulletScript.Init(damage,
        -1,
        Vector3.zero,
        criticalChancePercent:criticalChancePercent, 
        criticalDamagePercent:criticalDamagePercent);
    }
}
