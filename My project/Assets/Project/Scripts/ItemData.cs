using System.Collections;
using System.Collections.Generic;
using Data.WeaponData;
using UnityEngine;
using static Weapon;

[System.Serializable]
public class ItemDataInfo
{
    public WeaponId itemId;
    public int curLevel;
    public int maxLevel => damages.Length;
    public float[] damages;
    public int[] counts;
    public int[] ranges;
    public float baseCooldown;
    public float baseDamage;
    public int baseCount;
}
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Bomb, Raser, Breath, HGDClone, Stick, Floor, Glove, Shoe, Heal, Dagger, Pet }
    public ItemDataInfo itemDataInfo;
    
    [Header("# Main Info")]
    public ItemType itemType;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;


    [Header("# Weapon")]
    public GameObject projectile;

}
