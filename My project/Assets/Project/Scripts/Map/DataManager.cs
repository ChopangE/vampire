using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.WeaponData;
using Manager;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DataManager : MonoBehaviour
{
    private const string itemPath = "Items";
    private const string passiveItemPath = "Items/Passive";  // 패시브 아이템 경로 추가

    public List<ItemData> items = new List<ItemData>();

    public bool isLoaded = false;

    private void Awake()
    {
        InitializeItems();
        
        if(Global.UserDataManager.storage.itemDataInfoList.Count == 0)
        {
            LoadData();
        }
        LoadUserData().Forget();
    }

    private void InitializeItems()
    {
        items = new List<ItemData>();
        items.AddRange(Resources.LoadAll<ItemData>(itemPath));
        items.AddRange(Resources.LoadAll<ItemData>(passiveItemPath));
    }

    public void LoadData()
    {
        LoadInGameDatas();
        InitializeNewGameData();
        SaveToUserStorage();
    }

    private void InitializeNewGameData()
    {
        foreach (var item in items)
        {
            item.itemDataInfo.curLevel = item.itemDataInfo.itemId == WeaponId.Dagger ? 1 : 0;
            item.Reset();
        }
    }

    private void SaveToUserStorage()
    {
        Global.UserDataManager.storage.passiveItemDataInfoList = items.Where(item => item.itemType == ItemType.Passive)
            .Select(item => item.passiveItemDataInfo).ToList();
        Global.UserDataManager.storage.itemDataInfoList = items.Where(item => item.itemType != ItemType.Passive)
            .Select(item => item.itemDataInfo).ToList();
        Global.UserDataManager.Save();
    }


    private void LoadInGameDatas()
    {
        InitializeItems();
    }

    public PassiveItemDataInfo GetPassiveItemDataInfo(PassiveId passiveId)
    {
        return Global.UserDataManager.storage.passiveItemDataInfoList.Find(item => item.passiveId == passiveId);
    }

    public ItemDataInfo GetItemDataInfo(ItemData itemData)
    {
        return Global.UserDataManager.storage.itemDataInfoList
            .FirstOrDefault(info => itemData.itemDataInfo.itemId == info.itemId);
    }

    public async UniTask<ItemDataInfo> GetItemDataInfo(WeaponId itemId)
    {
        if (!isLoaded) await WaitForLoading();

        var info = Global.UserDataManager.storage.itemDataInfoList
            .FirstOrDefault(info => itemId == info.itemId);

        if (info == null)
        {
            Debug.LogWarning($"WeaponId {itemId}에 해당하는 ItemDataInfo를 찾을 수 없습니다.");
        }
        return info;
    }

    public ItemData[] GetNotMaxLevelItems()
    {
        return items.Where(item => {
            var info = GetItemDataInfo(item);
            return info != null && info.curLevel < info.maxLevel;
        }).ToArray();
    }

    public ItemData[] GetMaxLevelItems()
    {
        return items.Where(item => {
            var info = GetItemDataInfo(item);
            return info != null && info.curLevel >= info.maxLevel;
        }).ToArray();
    }
    public void SetPassiveItemLevel(ItemData itemData, int level)
    {
        var targetItem = Global.UserDataManager.storage.passiveItemDataInfoList
            .FirstOrDefault(item => item.passiveId == itemData.passiveItemDataInfo.passiveId);
            
    }

    public void SetWeaponItemLevel(ItemData itemData, int level)
    {
        var targetItem = Global.UserDataManager.storage.itemDataInfoList
            .FirstOrDefault(item => item.itemId == itemData.itemDataInfo.itemId);
            
        if (targetItem != null)
        {
            targetItem.curLevel = level;
            Global.UserDataManager.Save();
        }
    }

    public void SaveWeaponData(ItemData itemData)
    {
        var targetItem = Global.UserDataManager.storage.itemDataInfoList
            .FirstOrDefault(item => item.itemId == itemData.itemDataInfo.itemId);
            
        if (targetItem != null)
        {
            UpdateWeaponStats(targetItem, itemData.itemDataInfo);
            Global.UserDataManager.Save();
        }
    }

    private void UpdateWeaponStats(ItemDataInfo target, ItemDataInfo source)
    {
        target.curCoolDown = source.curCoolDown;
        target.curDuration = source.curDuration;
        target.curDamage = source.curDamage;
        target.curCount = source.curCount;
        target.curRange = source.curRange;
        target.curCriticalChancePercent = source.curCriticalChancePercent;
        target.curCriticalDamagePercent = source.curCriticalDamagePercent;
    }

    private async UniTaskVoid LoadUserData()
    {
        await UniTask.WaitUntil(() => Global.UserDataManager != null);

        isLoaded = true;
    }

    private async UniTask WaitForLoading()
    {
        while (!isLoaded)
        {
            await UniTask.Yield();
        }
    }

    public void ResetData()
    {
        Global.UserDataManager.storage.itemDataInfoList.Clear();
        Global.UserDataManager.storage.passiveItemDataInfoList.Clear();
        Global.UserDataManager.Save();
        LoadData();
    }

    public void SaveData()
    {
        // 모든 아이템의 현재 상태를 저장
        foreach (var item in items)
        {
            if (item.itemType == ItemType.Passive)
            {
                var targetItem = Global.UserDataManager.storage.passiveItemDataInfoList
                    .FirstOrDefault(info => info.passiveId == item.passiveItemDataInfo.passiveId);
                
                if (targetItem != null)
                {
                    targetItem.curLevel = item.passiveItemDataInfo.curLevel;
                }
            }
            else
            {
                SaveWeaponData(item);
            }
        }
        
        Global.UserDataManager.Save();
    }
}