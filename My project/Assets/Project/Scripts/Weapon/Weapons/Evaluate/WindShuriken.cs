using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Data;
using Manager;


public class WindShuriken : BulletWeapon
{
    public override async UniTask Init()
    {
        await base.Init();

        // Wind_Shuriken 업적 클리어 (최초 획득 시)
        var userDataManager = Manager.Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (!killSaveData.ContainsKey("Wind_Shuriken"))
            {
                killSaveData["Wind_Shuriken"] = 1;
                userDataManager.Save();
                var achievementsManager = GameObject.FindObjectOfType<AchievementsManager>();
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID("Wind_Shuriken");
                }
            }
        }
    }
    public override void SpawnBullet()
    {
        Global.SoundManager.PlaySFX(SFXEnum.WindShuriken);
        Transform eliteOrBoss = player.scan.GetNearstEliteOrBoss();
        if (eliteOrBoss != null)
        {
            // 보스나 엘리트를 향해 발사
            Vector3 targetPos = eliteOrBoss.position;
            Vector3 dir = (targetPos - transform.position).normalized;
            
            Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            bullet.transform.localScale = Vector3.one * 2;
            var bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Init(damage, -1, dir, 
            criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
            bulletScript.rb.velocity = dir * 7f;
        }
        else
        {
            // 보스나 엘리트가 없으면 랜덤 방향으로 발사
            float randomAngle = Random.Range(0f, 360f);
            Vector3 dir = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;

            Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            bullet.transform.localScale = Vector3.one * 2;
            var bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Init(damage, -1, dir, 
            criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
            bulletScript.rb.velocity = dir * 7f;
        }
    }
}
