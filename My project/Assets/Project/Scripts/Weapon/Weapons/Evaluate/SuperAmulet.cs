using UnityEngine;
using Cysharp.Threading.Tasks;
using Manager;
public class SuperAmulet : BulletWeapon
{
    public override async UniTask Init()
    {
        await base.Init();

        // Reinforced_Explosive_Talisman 업적 클리어 (최초 획득 시)
        var userDataManager = Manager.Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (!killSaveData.ContainsKey("Reinforced_Explosive_Talisman"))
            {
                killSaveData["Reinforced_Explosive_Talisman"] = 1;
                userDataManager.Save();
                var achievementsManager = GameObject.FindObjectOfType<AchievementsManager>();
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID("Reinforced_Explosive_Talisman");
                }
            }
        }
    }
    public override void SpawnBullet()
    {
        // 랜덤한 각도 생성 (0~360도)
        float randomAngle = Random.Range(0f, 360f);
        Vector3 dir = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;

        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.GetChild(0).GetComponent<Bullet>().Init(damage, -1, dir, 
        criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
    }
}
