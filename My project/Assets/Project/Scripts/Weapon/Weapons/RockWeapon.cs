using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Manager;
using UnityEngine;

public class RockWeapon : Weapon
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override void ExecuteAttack()
    {
        if (!player.scan.nearestTarget) return;
        Global.SoundManager.PlaySFX(SFXEnum.ThrowRock);
        Vector3 targetPos = player.scan.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir, criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
    }

}
