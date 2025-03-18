using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using SO;
using Manager.InGame;
using Data;
using Manager;

public class Bowling : Weapon
{
    public override async UniTask Init()
    {
        await base.Init();
        maxCooldown = 5f;
        remainingCooldown = maxCooldown;
    }
    public override void ExecuteAttack()
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
