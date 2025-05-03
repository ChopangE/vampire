using System.Collections;
using System.Collections.Generic;
using Data.WeaponData;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemType type;
    public float rate;


    public void Init(ItemData data) {
        name = "Gear" + data.itemDataInfo.itemId;
        transform.parent = GameManager.Instance.player.transform;
        transform.localPosition = Vector3.zero;

        type = data.itemType;
        rate = data.itemDataInfo.baseDamage;
        ApplayGear();
    }
    public void LevelUP(float rate) {
        this.rate = rate;
        ApplayGear();
    }

    void ApplayGear() {
        switch (type) {
            case ItemType.Glove:
                RateUp();
                break;
            case ItemType.Shoe:
                SpeedUp();

                break;
        }
    }

    void RateUp() {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons) {
            switch (weapon.id) {
                case WeaponId.Wind:
                    weapon.maxCooldown = 150 + (150 * rate);
                    break;
                case WeaponId.Rock:
                    weapon.maxCooldown = 0.5f * (1f - rate);
                    break;
            }
        }
    }
    void SpeedUp() {
        float speed = 3;
        GameManager.Instance.player.speed = speed + speed * rate;
    }
}
