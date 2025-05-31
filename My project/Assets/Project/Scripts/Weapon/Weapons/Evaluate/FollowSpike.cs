using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FollowSpike : BulletWeapon
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override void SpawnBullet()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        bullet.rotation = Quaternion.identity;
        bullet.GetComponent<MoveSpikeBullet>().Init(damage, -100, Vector3.zero, duration: duration, criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
    }
}
