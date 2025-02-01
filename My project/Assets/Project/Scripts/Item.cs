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
        if (data.itemType != ItemType.Passive)
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
            var passive = Global.DataManager.GetPassiveItemDataInfo(data.passiveItemDataInfo.passiveId);
            Level = passive.curLevel;
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
            case ItemType.Passive:
                desc = string.Format(data.itemDesc);
                textDesc.text = desc;
                return;

            case ItemType.Pet:
                textName.text = string.Format("{0} / 3", Level);
                desc = string.Format(data.itemDesc);
                textDesc.text = desc;
                return;

        }

        var upgrade = Global.UpgradeManager.GetRandomSkillUpgrade(data.excludeUpgradeList);
        var baseDataInfo = weapon._data.itemDataInfo;
        if(upgrade != null)
        {
            prevUpgradeName = upgrade.upgradeName;
            if(upgrade.upgradeName == UpgradeName.Damage)
            {
                prevDamageUpgradeValues = Global.UpgradeManager.GetDamageUpgradeValues();

                var damageDesc = (weapon._data.itemDataInfo.baseDamage * (1 + prevDamageUpgradeValues.damagePercent)) - (weapon._data.itemDataInfo.baseDamage);
                desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " + {0}", damageDesc);
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
                        prevUpgradeValue = value;
                        break;
                    case UpgradeName.Cooldown: // 곱연산
                        prevUpgradeValue = baseDataInfo.curCoolDown - (value * baseDataInfo.curCoolDown);
                        desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " - {0:F2}s", prevUpgradeValue);
                        break;
                    case UpgradeName.Duration: // 곱연산
                        prevUpgradeValue = (value * baseDataInfo.curDuration) - baseDataInfo.curDuration;
                        desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " + {0:F2}s", prevUpgradeValue);
                        break;
                    case UpgradeName.Range: // 곱연산
                        prevUpgradeValue = (value * baseDataInfo.curRange) - baseDataInfo.curRange;
                        desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " + {0:F2}%", prevUpgradeValue);
                        break;
                    default:
                        prevUpgradeValue = (value * 100) - 100;
                        desc = string.Format(LocalizationManager.GetTranslation(upgrade.upgradeNameKey) + " + {0:F2}%", prevUpgradeValue);
                        break;
                }
            }
            
        }
        // if(Level == 0 && data.itemType != ItemData.ItemType.Heal && data.itemType != ItemData.ItemType.Pet)
        //     textDesc.text = "";
        // else
        textDesc.text = desc;
    }


    private void CheckInteractable()
    {
        if (data.itemType == ItemType.Passive)
        {
            IsInteractable = true;

            if(data.passiveItemDataInfo.curLevel >= data.passiveItemDataInfo.maxLevel)
            {
                IsInteractable = false;
            }
            return;
        }
        var weapons = GameManager.Instance.weaponController.ActiveWeapons;
        if (weapons.Count >= GameManager.Instance.weaponController.maxActiveWeaponCount)
        {
            if(data.isEvaluateWeapon)
            {
                IsInteractable = true;
            }
            else
            {
                IsInteractable = false;
            }
        }
        else
        {
            IsInteractable = true;
        }
    }

    public void OnClick()
    {

        // Heal 타입 먼저 처리
        if (data.itemType == ItemType.Passive)
        {
            data.passiveItemDataInfo.curLevel++;
            Global.DataManager.SetPassiveItemLevel(data, data.passiveItemDataInfo.curLevel);
            GameManager.PassiveManager.SetPassiveItem(data.passiveItemDataInfo);
            return;
        }



        InitializeWeapon(GameManager.Instance.weaponController.Weapons.ToArray());
        switch (data.itemType)
        {
            case ItemType.Melee:
            case ItemType.Range:
            case ItemType.Bomb:
            case ItemType.Raser:
            case ItemType.Breath:
            case ItemType.HGDClone:
            case ItemType.Stick:
            case ItemType.Pet:
            case ItemType.Floor:

            case ItemType.Dagger:
            case ItemType.ShadowPlayer:
                UpgradeWeapon();
                Level++;
                break;

            case ItemType.Shoe:
                HandleGear();
                Level++;
                break;

        }

        Global.DataManager.SetWeaponItemLevel(data, Level);
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