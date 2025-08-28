using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Spike : BulletWeapon
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override void SpawnBullet()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0);
        bullet.rotation = Quaternion.identity;
        bullet.GetComponent<Trap>().Init(damage, -100, Vector3.zero, duration: duration, size: size, criticalChancePercent: criticalChancePercent, criticalDamagePercent: criticalDamagePercent);
    }
}
