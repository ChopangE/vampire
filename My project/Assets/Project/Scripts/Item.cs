using System.Collections;
using System.Collections.Generic;
using Manager;
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

    private WeaponController weaponController;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;

        weaponController = GameManager.Instance.player.GetComponent<WeaponController>();
    }

    void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);

        switch (data.itemType)
        {
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
            case ItemData.ItemType.Heal:
                GameManager.Instance.health = GameManager.Instance.maxHealth;
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }

    }


    public void OnClick()
    {
        Weapon[] weapons = GameManager.Instance.player.GetComponentsInChildren<Weapon>(true);
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                InitializeWeapon(weapons);
                UpgradeWeapon();
                level++;
                break;
            case ItemData.ItemType.Bomb:
            case ItemData.ItemType.Raser:
            case ItemData.ItemType.Breath:
            case ItemData.ItemType.HGDClone:
            case ItemData.ItemType.Stick:
                InitializeWeapon(weapons);
                UpgradeWeapon();
                level++;
                break;
            case ItemData.ItemType.Floor:
                HandleFloorWeapon();
                level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                HandleGear();
                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.Instance.health = GameManager.Instance.maxHealth;
                break;
        }


        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }

    private void InitializeWeapon(Weapon[] weapons)
    {
        weapon = weapons[(int)data.itemType];
        weapon.gameObject.SetActive(true);
        weapon.Init(data);
        weaponController.ActivateWeapon(weapon);
    }

    private void UpgradeWeapon(bool includeSizeUpgrade = false)
    {
        if (weapon == null)
        {
            weapon = GameManager.Instance.player.GetComponentsInChildren<Weapon>(true)[(int)data.itemType];
        }

        float nextDamage = data.baseDamage * (1 + data.damages[level]);
        int nextCount = data.counts[level];

        weapon.LevelUp(nextDamage, nextCount);

        if (includeSizeUpgrade)
        {
            float nextSize = 1f + (0.1f * level);
            weapon.transform.localScale = Vector3.one * nextSize;
        }
    }

    private void HandleFloorWeapon()
    {
        if (level == 0)
        {
            floorWeapon = GameManager.Instance.player.GetComponentInChildren<FloorWeapon>(true);
            floorWeapon.gameObject.SetActive(true);
            floorWeapon.Init(data);
        }
        else
        {
            if (floorWeapon == null) 
                floorWeapon = GameManager.Instance.player.GetComponentInChildren<FloorWeapon>(true);
            
            float nextDamage = data.baseDamage * (1 + data.damages[level]);
            floorWeapon.LevelUp(nextDamage);
        }
    }

    private void HandleGear()
    {
        if (level == 0)
        {
            gear = new GameObject().AddComponent<Gear>();
            gear.Init(data);
        }
        else
        {
            gear.LevelUP(data.damages[level]);
        }
    }

    private void OnWeaponActivated(int weaponIndex)
    {
        if ((int)data.itemType == weaponIndex)
        {
            // 해당 무기가 활성화되었을 때의 처리
            Debug.Log($"Weapon {data.itemName} has been activated!");
        }
    }
}
