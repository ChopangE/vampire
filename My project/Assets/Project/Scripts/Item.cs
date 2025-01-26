using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.WeaponData;
using I2.Loc;
using Manager;
using Manager.InGame;
using SO;
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
    private bool _isMaxLevel;
    [Binding]
    public bool IsMaxLevel
    {
        get => _isMaxLevel;
        set
        {
            _isMaxLevel = value;
            OnPropertyChanged(nameof(IsMaxLevel));
        }
    }
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
    private DamageUpgradeValues prevDamageUpgradeValues;
    private float prevUpgradeValue;
    private UpgradeName prevUpgradeName;

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


        UpdateDesc();

        CheckInteractable();
    }

    private void UpdateDesc()
    {
        prevDamageUpgradeValues = null;
        prevUpgradeValue = 0;
        prevUpgradeName = UpgradeName.Damage;

        var desc = data.itemDesc;
        switch(data.itemType)
        {
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                desc = string.Format(data.itemDesc, data.itemDataInfo.baseDamage);
                textDesc.text = desc;
                return;
            case ItemData.ItemType.Heal:
                desc = string.Format(data.itemDesc, data.itemDataInfo.baseDamage);
                textDesc.text = desc;
                return;
            case ItemData.ItemType.Pet:
                textName.text = string.Format("{0} / 3", Level);
                desc = string.Format(data.itemDesc);
                textDesc.text = desc;
                return;
        }

        var upgrade = Global.UpgradeManager.GetRandomSkillUpgrade(data.excludeUpgradeList);
        if(upgrade != null)
        {
            prevUpgradeName = upgrade.upgradeName;
            if(upgrade.upgradeName == UpgradeName.Damage)
            {
                prevDamageUpgradeValues = Global.UpgradeManager.GetDamageUpgradeValues();
                desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " + {0}%", prevDamageUpgradeValues.damagePercent * 100);
                desc += string.Format("\n" + LocalizationManager.GetTranslation("Passive/Name/CritRateName") + " + {0}%", prevDamageUpgradeValues.critChancePercent * 100);
                desc += string.Format("\n" + LocalizationManager.GetTranslation("Passive/Name/CritDamageName") + " + {0}%", prevDamageUpgradeValues.critDamagePercent * 100);
            }else
            {
                var value = Global.UpgradeManager.GetUpgradeValue(upgrade.upgradeName);
                switch (upgrade.upgradeName)
                {
                    case UpgradeName.Projectiles:
                    case UpgradeName.PierceLimit:
                        desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " + {0}", value);
                        break;
                    case UpgradeName.Cooldown:
                        desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " - {0}%", (1 - value) * 100);
                        break;
                    case UpgradeName.Range:
                        desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " + {0}%", value);
                        break;
                    case UpgradeName.Duration:
                        desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " + {0}%", value * 100);
                        break;
                    default:
                        desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " + {0}%", value * 100);
                        break;
                }
            }
            
        }
        textDesc.text = desc;
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
            case ItemData.ItemType.Dagger:
            case ItemData.ItemType.ShadowPlayer:
                UpgradeWeapon();
                Level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                HandleGear();
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

    private void UpgradeWeapon()
    {
        if (weapon == null)
        {
            GetWeapon();
        }
        weapon.LevelUp(prevUpgradeName, prevUpgradeValue, prevDamageUpgradeValues);
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
            gear.LevelUP(data.itemDataInfo.baseDamage);
        }
    }

}
