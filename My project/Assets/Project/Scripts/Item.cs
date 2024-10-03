using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public FloorWeapon floorWeapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake() {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    void OnEnable() {
        textLevel.text = "Lv." + (level + 1);
    
        switch (data.itemType) {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
            case ItemData.ItemType.Bomb:           
            case ItemData.ItemType.Raser:
            case ItemData.ItemType.Breath:
            case ItemData.ItemType.Floor:
            case ItemData.ItemType.HGDClone:
            case ItemData.ItemType.Stick:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }
        
    }
    

    public void OnClick()
    {
        Weapon[] weapons = GameManager.Instance.player.GetComponentsInChildren<Weapon>(true);
        switch (data.itemType) {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
            case ItemData.ItemType.Bomb:
            case ItemData.ItemType.Raser:
            case ItemData.ItemType.Breath:
            case ItemData.ItemType.HGDClone:
            case ItemData.ItemType.Stick:
                if (level == 0)
                {
                    weapon = weapons[(int)data.itemType];
                    weapon.gameObject.SetActive(true);
                    weapon.Init(data);
                }

                else {
                    if(weapon == null) {
                        weapon = weapons[(int)data.itemType];
                    }
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);

                }
                level++;

                break;
            case ItemData.ItemType.Floor:
                if(level == 0) {
                    floorWeapon = GameManager.Instance.player.GetComponentInChildren<FloorWeapon>(true);
                    floorWeapon.gameObject.SetActive(true);
                    floorWeapon.Init(data);
                }
                else {
                    if (floorWeapon == null) floorWeapon = GameManager.Instance.player.GetComponentInChildren<FloorWeapon>(true);
                    float nextDamage = data.baseDamage;
                    nextDamage += data.baseDamage * data.damages[level];
                    floorWeapon.LevelUp(nextDamage);
                }
                level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if(level == 0) {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else {
                    float nextRate = data.damages[level];
                    gear.LevelUP(nextRate);
                }
                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.Instance.health = GameManager.Instance.maxHealth;
                break;
        }


        if(level == data.damages.Length) {
            GetComponent<Button>().interactable = false;
        }
    }
    
}
