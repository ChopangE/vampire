using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FloorWeapon : BulletWeapon
{
    protected override void Awake() {
        player = GameManager.Instance.player;
    }

    
    public override async UniTask Init() {
        await base.Init();
    }

    public void LevelUp(float damage) {
        this.damage = damage;

        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);

    }
    public override void SpawnBullet() {
        
    }
}
