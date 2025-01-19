using System.Collections;
using System.Collections.Generic;
using Data.WeaponData;
using Sirenix.OdinInspector;
using SO;
using UnityEngine;
using static Weapon;

[System.Serializable]
public class ItemDataInfo
{
    public WeaponId itemId;
    public int curLevel;
    public int maxLevel = 10;
    public float baseCooldown;
    public float baseDamage;
    public int baseCount;
    public float baseRange;
}
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Bomb, Raser, Breath, HGDClone, Stick, Floor, Glove, Shoe, Heal, Dagger, Pet,
    ShadowPlayer }
    public ItemDataInfo itemDataInfo;
    
    [Header("# Main Info")]
    public ItemType itemType;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;


    [Header("# Weapon")]
    public GameObject projectile;

    [LabelText("제외할 업그레이드 속성")]
    public List<SkillUpgradeSO> excludeUpgradeList = new List<SkillUpgradeSO>();

}
