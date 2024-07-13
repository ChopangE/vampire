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
        // Basic Set
        //name = "Weapon " + data.itemId;
        //transform.parent = player.transform;
        //transform.localPosition = Vector3.zero;

        // Property Set
        id = data.itemId;
        damage = data.baseDamage;
    }

    public void LevelUp(float damage) {
        this.damage = damage;

        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);

    }
}
