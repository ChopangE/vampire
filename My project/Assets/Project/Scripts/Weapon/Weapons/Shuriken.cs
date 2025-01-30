using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class Shuriken : Weapon
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override void ExecuteAttack()
    {
        if (!player.scan.nearestTarget) return;

        // Vector3 targetPos = player.scan.nearestTarget.position;
        // Vector3 dir = (targetPos - transform.position).normalized;
        // Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;

        // bullet.position = transform.position;
        // bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        // bullet.GetComponent<Bullet>().Init(damage, count, dir, criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
        // 랜덤한 각도 생성 (0~360도)
        float randomAngle = Random.Range(0f, 360f);
        Vector3 dir = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;

        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        var bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Init(damage, pierce, dir, 
        criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
        bulletScript.rb.velocity = dir * 7f;
    }
}
