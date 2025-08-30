using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FollowSpike : BulletWeapon
{
    public override async UniTask Init()
    {
        await base.Init();

        // Moving_Spike_Trap 업적 클리어 (최초 획득 시)
        var userDataManager = Manager.Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (!killSaveData.ContainsKey("Moving_Spike_Trap"))
            {
                killSaveData["Moving_Spike_Trap"] = 1;
                userDataManager.Save();
                var achievementsManager = GameObject.FindObjectOfType<AchievementsManager>();
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID("Moving_Spike_Trap");
                }
            }
        }
    }
    public override void SpawnBullet()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        
        bullet.position = transform.position + new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0);
        bullet.rotation = Quaternion.identity;
        bullet.GetComponent<MoveSpikeBullet>().Init(damage, -100, Vector3.zero, duration: duration, size: size, criticalChancePercent: criticalChancePercent, criticalDamagePercent: criticalDamagePercent);
    }
}
