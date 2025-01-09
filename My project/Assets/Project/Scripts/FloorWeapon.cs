using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FloorWeapon : Weapon
{
    protected override void Awake() {
        player = GameManager.Instance.player;
    }

    
    public override async UniTaskVoid Init(ItemData data) {
        base.Init(data).Forget();
        await UniTask.Yield();
    }

    public void LevelUp(float damage) {
        this.damage = damage;

        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);

    }
    public override void ExecuteAttack() {
        
    }
}
