using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Melee
{
    public override void Init(ItemData data)
    {
        base.Init(data);
        maxCooldown = 1f;
        remainingCooldown = maxCooldown;
    }
    public override void ExecuteAttack()
    {
        SetAttackDirection();
        GameObject sword = SpawnSword();
        ConfigureSword(sword);
    }
}