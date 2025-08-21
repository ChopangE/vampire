using System.Collections;
using System.Collections.Generic;
using Data.WeaponData;
using Sirenix.OdinInspector;
using SO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ItemDataInfo
{
    public WeaponId itemId;
    public int curLevel;
    public int maxLevel;
    public float baseCooldown;
    public float baseDamage;
    public int baseCount;
    public float baseRange;
    public float baseDuration;
    public int basePierce;

    [FoldoutGroup("현재 데이터")]
    public float curCoolDown = 0;
    [FoldoutGroup("현재 데이터")]
    public float curDamage = 0;
    [FoldoutGroup("현재 데이터")]
    public int curCount = 0;
    [FoldoutGroup("현재 데이터")]
    public float curRange = 0;
    [FoldoutGroup("현재 데이터")]
    public float curDuration = 0;
    [FoldoutGroup("현재 데이터")]
    public float curCriticalDamagePercent = 0;
    [FoldoutGroup("현재 데이터")]
    public float curCriticalChancePercent = 0;
    [FoldoutGroup("현재 데이터")]
    public int curPierce = 0;

}

public enum PassiveId
{
    Health,
    Speed,
    Damage,
    ExpGainIncrease,        // 경험치 획득량 증가
    ExpGainRangeIncrease,   // 경험치 획득 범위 증가
    Defense,                // 방어력 증가
    CriticalDamage,         // 치명타 데미지 증가
    CriticalChance,         // 치명타 확률 증가
}
[System.Serializable]
public class PassiveItemDataInfo
{
    public PassiveId passiveId;
    public float passiveValue;
    public int curLevel;
    public int maxLevel = 1000;
}

public enum ItemType { Melee, Range, Bomb, Raser, Breath, HGDClone, Stick, Floor, Glove, Shoe, Passive, Dagger, Pet,
ShadowPlayer}
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public ItemDataInfo itemDataInfo;

    public PassiveItemDataInfo passiveItemDataInfo;
    
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

    [BoxGroup("진화무기 세팅")] [Button("진화무기인지 자동 체크")] 
    public void CheckEvaluateWeapon()
    {
        if(_prevItemData != null)
        {
            isEvaluateWeapon = true;
        }else
        {
            isEvaluateWeapon = false;
        }
    }
    [LabelText("진화무기인가")] public bool isEvaluateWeapon = false;
    [BoxGroup("진화무기 세팅")] [Button("수치 초기화")] 
    public void Reset()
    {
        itemDataInfo.curPierce = 0;
        itemDataInfo.curCoolDown = 0;
        itemDataInfo.curDamage = 0;
        itemDataInfo.curCount = 0;
        itemDataInfo.curRange = 0;
        itemDataInfo.curDuration = 0;
        itemDataInfo.curCriticalDamagePercent = 0;
        itemDataInfo.curCriticalChancePercent = 0;
        // itemDataInfo.baseCount = 1;
        // itemDataInfo.maxLevel = 9;
        // itemDataInfo.baseRange = 1f; 
        // itemDataInfo.baseDuration = 1f;
    }
    [Button("이름 변경")]
    private void ChangeName()
    {
        #if UNITY_EDITOR
        if (!string.IsNullOrEmpty(itemName))
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(assetPath))
            {
                this.name = itemName;
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.RenameAsset(assetPath, itemName);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }
        #endif
    }
    [BoxGroup("진화무기 세팅")] public ItemData _prevItemData;
    [BoxGroup("진화무기 세팅")] public ItemData _nextItemData;
    [BoxGroup("진화무기 세팅")] [Button("기존 무기 데이터 상속 (위의 데이터에 해당 ItemData 할당 필요)")]
    public void InheritValues()
    {
        if(_prevItemData == null)
        {
            Debug.LogError("기존 무기 데이터가 할당되지 않았습니다.");
            return;
        }else
        {
            Debug.Log($"기존 무기 데이터 {_prevItemData.itemName} 상속");
        }

        // 이전 무기의 제외할 업그레이드 리스트를 확인하여 해당 속성들은 상속하지 않음
        var excludedUpgrades = new HashSet<SO.UpgradeName>();
        if (_prevItemData.excludeUpgradeList != null)
        {
            foreach (var upgrade in _prevItemData.excludeUpgradeList)
            {
                if (upgrade != null)
                {
                    excludedUpgrades.Add(upgrade.upgradeName);
                }
            }
        }

        // 제외되지 않은 속성들만 상속
        if (!excludedUpgrades.Contains(SO.UpgradeName.Projectiles))
            itemDataInfo.baseCount = _prevItemData.itemDataInfo.baseCount;
        
        itemDataInfo.maxLevel = _prevItemData.itemDataInfo.maxLevel;
        
        if (!excludedUpgrades.Contains(SO.UpgradeName.Range))
            itemDataInfo.baseRange = _prevItemData.itemDataInfo.baseRange;
        
        if (!excludedUpgrades.Contains(SO.UpgradeName.Damage))
            itemDataInfo.baseDamage = _prevItemData.itemDataInfo.baseDamage;
        
        if (!excludedUpgrades.Contains(SO.UpgradeName.Cooldown))
            itemDataInfo.baseCooldown = _prevItemData.itemDataInfo.baseCooldown;
        
        if (!excludedUpgrades.Contains(SO.UpgradeName.Duration))
            itemDataInfo.baseDuration = _prevItemData.itemDataInfo.baseDuration;
        
        if (!excludedUpgrades.Contains(SO.UpgradeName.PierceLimit))
            itemDataInfo.basePierce = _prevItemData.itemDataInfo.basePierce;
    }
    [Button("저장")]
    public void Save()
    {
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }
}
