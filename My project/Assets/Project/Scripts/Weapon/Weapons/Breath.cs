using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breath : Weapon
{
    public override void Init(ItemData data)
    {
        base.Init(data);
        maxCooldown = 0.5f;
    }
    public override void ExecuteAttack()
    {   
        Vector3 dir = GetPlayerDirection();
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + dir * 3f;
    }
}
