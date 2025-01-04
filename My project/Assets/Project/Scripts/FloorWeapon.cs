using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorWeapon : Weapon
{
    protected override void Awake() {
        player = GameManager.Instance.player;
    }

    
    public override void Init(ItemData data) {
        base.Init(data);
    }

    public void LevelUp(float damage) {
        this.damage = damage;

        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);

    }
    public override void ExecuteAttack() {
        
    }
}
