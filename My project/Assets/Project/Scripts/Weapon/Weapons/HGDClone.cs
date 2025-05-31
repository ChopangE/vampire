using UnityEngine;
using Cysharp.Threading.Tasks;

public class HGDClone : BulletWeapon
{
    public override async UniTask Init()
    {
        await base.Init();
        maxCooldown = 7f;
    }
    public override void SpawnBullet()
    {
        Vector3 dir = GetPlayerDirection();
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        bullet.GetComponent<Bullet>().Init(damage, 100, dir, criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
    }
}
