using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Data;
using Manager;

public class Dagger : Melee
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override void SpawnBullet()
    {
        SetAttackDirection();
        Global.SoundManager.PlaySFX(SFXEnum.DaggerSwing);
        GameObject sword = SpawnSword();
        ConfigureSword(sword, null, true);
    }
}