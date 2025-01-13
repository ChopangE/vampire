using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Dagger : Melee
{
    public override async UniTaskVoid Init(ItemData data)
    {
        base.Init(data).Forget();
        await UniTask.Yield();
        maxCooldown = 1f;
        remainingCooldown = maxCooldown;
    }
    public override void ExecuteAttack()
    {
        SetAttackDirection();
        GameObject sword = SpawnSword();
        ConfigureSword(sword, null, true);
    }
}