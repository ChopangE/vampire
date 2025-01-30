using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Dagger : Melee
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override void ExecuteAttack()
    {
        SetAttackDirection();
        GameObject sword = SpawnSword();
        ConfigureSword(sword, null, true);
    }
}