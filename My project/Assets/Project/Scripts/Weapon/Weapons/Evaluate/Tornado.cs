using Cysharp.Threading.Tasks;
using Data;
using Manager;
using UnityEngine;
public class Tornado : BulletWeapon
{
    private const string TORNADO = "Tornado"; 
    public override async UniTask Init()
    {
        await base.Init();

        // Tornado 업적 클리어 (최초 획득 시)
        var userDataManager = Manager.Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (!killSaveData.ContainsKey(TORNADO))
            {
                killSaveData[TORNADO] = 1;
                userDataManager.Save();
                var achievementsManager = GameObject.FindObjectOfType<AchievementsManager>();
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID(TORNADO);
                }
            }
        }
    }
    public override void SpawnBullet()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        bullet.rotation = Quaternion.identity;
        bullet.GetComponent<Bullet>().Init(
            damage,
            -1,
            Vector3.zero,
            criticalChancePercent: criticalChancePercent,
            criticalDamagePercent: criticalDamagePercent,
            duration: duration
        );
    }
}