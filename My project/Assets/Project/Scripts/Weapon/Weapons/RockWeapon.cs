using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockWeapon : Weapon
{
    public override void Init(ItemData data)
    {
        base.Init(data);
        maxCooldown = 0.75f;
    }
    public override void ExecuteAttack()
    {
        if (!player.scan.nearestTarget) return;

        Vector3 targetPos = player.scan.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }

}
