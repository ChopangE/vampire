using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorWeapon : MonoBehaviour
{
    public int id;
    public float damage;

    protected float timer;
    protected Player player;

    void Awake() {
        player = GameManager.Instance.player;
    }

    
    public virtual void Init(ItemData data) {
        id = data.itemId;
        damage = data.baseDamage;
    }

    public void LevelUp(float damage) {
        this.damage = damage;

        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);

    }
}
