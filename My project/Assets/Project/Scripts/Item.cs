using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.WeaponData;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld;
using UnityWeld.Binding;

[Binding]
public class Item : ViewModel
{
    private bool _isInteractable = true;
    [Binding]
    public bool IsInteractable
    {
        get => _isInteractable;
        set
        {
            _isInteractable = value;
            OnPropertyChanged(nameof(IsInteractable));
        }
    }
    public ItemData data;
    private int _level;
    [Binding]
    public int Level
    {
        get => _level;
        set
        {
            _level = value;
            OnPropertyChanged(nameof(Level));
        }
    }
    public Weapon weapon;
    public FloorWeapon floorWeapon;
    public Gear gear;

    private Sprite _icon;
    [Binding]
    public Sprite Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            OnPropertyChanged(nameof(Icon));
        }
    }
    Text textLevel;
    Text textName;
    Text textDesc;

    private WeaponController weaponController;

    private void GetWeapon()
    {
        var weapons = GameManager.Instance.player.GetComponentInChildren<WeaponController>(true).Weapons;
        foreach (var obj in weapons)
        {
            if (obj.id == data.itemDataInfo.itemId)
            {
                weapon = obj;
                break;
            }
        }
    }
    void OnEnable()
    {
        if (data.itemType != ItemData.ItemType.Heal)
        {
            GetWeapon();
            if (weapon == null)
            {
                Debug.LogError($"Weapon {data.itemName} not found");
                return;
            }
            Level = weapon.level;
        }
        else
        {
            Level = 00;
        }
        Icon = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;

        weaponController = GameManager.Instance.player.GetComponent<WeaponController>();
        textLevel.text = "Lv." + (Level + 1);


        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.itemDataInfo.damages[Level] * 100, data.itemDataInfo.counts[Level]);
                break;
            case ItemData.ItemType.Bomb:
            case ItemData.ItemType.Raser:
            case ItemData.ItemType.Breath:
            case ItemData.ItemType.Floor:
            case ItemData.ItemType.HGDClone:
            case ItemData.ItemType.Stick:
            case ItemData.ItemType.ShadowPlayer:
                textDesc.text = string.Format(data.itemDesc, data.itemDataInfo.damages[Level] * 100);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.itemDataInfo.damages[Level] * 100);
                break;
            case ItemData.ItemType.Heal:
                textDesc.text = string.Format(data.itemDesc, data.itemDataInfo.damages[Level] * 100);
                break;
            case ItemData.ItemType.Dagger:
                textDesc.text = string.Format(data.itemDesc, data.itemDataInfo.damages[Level] * 100, data.itemDataInfo.ranges[Level]);
                break;
            case ItemData.ItemType.Pet:
                textName.text = string.Format("{0} / 3", Level);
                textDesc.text = string.Format(data.itemDesc);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }

        CheckInteractable();
    }

    private void CheckInteractable()
    {
        if (data.itemType == ItemData.ItemType.Heal)
        {
            IsInteractable = true;
            return;
        }
        var weapons = GameManager.Instance.weaponController.ActiveWeapons;
        if (weapons.Count >= GameManager.Instance.weaponController.maxActiveWeaponCount)
        {
            IsInteractable = false;
        }
        else
        {
            IsInteractable = true;
        }
    }

    public void OnClick()
    {

        // Heal 타입 먼저 처리
        if (data.itemType == ItemData.ItemType.Heal)
        {
            switch (data.itemDataInfo.itemId)
            {
                case WeaponId.Drink:
                    GameManager.Instance.health = GameManager.Instance.maxHealth;
                    break;
                case WeaponId.Shoe:
                    GameManager.Instance.player.speed *= (1 + 0.1f);
                    break;
                case WeaponId.Glove:
                    GameManager.Instance.player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);
                    break;
            }
            return;
        }

        InitializeWeapon(GameManager.Instance.weaponController.Weapons.ToArray());
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
            case ItemData.ItemType.Bomb:
            case ItemData.ItemType.Raser:
            case ItemData.ItemType.Breath:
            case ItemData.ItemType.HGDClone:
            case ItemData.ItemType.Stick:
            case ItemData.ItemType.Pet:
            case ItemData.ItemType.Floor:
            case ItemData.ItemType.ShadowPlayer:
                UpgradeWeapon();
                Level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                HandleGear();
                Level++;
                break;
            case ItemData.ItemType.Dagger:
                UpgradeWeapon(true);
                Level++;
                break;
        }

        DataManager.Instance.SetWeaponItemLevel(data, Level);
        if (Level >= data.itemDataInfo.maxLevel)
        {
            GetComponent<Button>().interactable = false;
        }
    }

    private void InitializeWeapon(Weapon[] weapons)
    {
        foreach (var obj in weapons)
        {
            if (obj.id == data.itemDataInfo.itemId)
            {
                weapon = obj;
                break;
            }
        }
        if (weapon == null)
        {
            Debug.LogError($"Weapon {data.itemName} not found");
            return;
        }
        weapon.gameObject.SetActive(true);
        weaponController.ActivateWeapon(weapon);
    }

    private void UpgradeWeapon(bool includeSizeUpgrade = false)
    {
        if (weapon == null)
        {
            GetWeapon();
        }

        float nextDamage = data.itemDataInfo.baseDamage * (1 + data.itemDataInfo.damages[Level]);
        int nextCount = data.itemDataInfo.baseCount + data.itemDataInfo.counts[Level];

        weapon.LevelUp(nextDamage, nextCount);

        if (includeSizeUpgrade)
        {
            float nextSize = 1f + (0.1f * Level);
            weapon.transform.localScale = Vector3.one * nextSize;
        }
    }

    private void HandleGear()
    {
        if (Level == 0)
        {
            gear = new GameObject().AddComponent<Gear>();
            gear.Init(data);
        }
        else
        {
            gear.LevelUP(data.itemDataInfo.damages[Level]);
        }
    }

}
