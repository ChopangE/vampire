using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Breath : Weapon
{
    public override async UniTask Init()
    {
        await base.Init();
        maxCooldown = 0.5f;
    }
    public override void ExecuteAttack()
    {   
        Vector3 dir = GetPlayerDirection();
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + dir * 3f;
    }
}
