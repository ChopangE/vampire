using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Data.WeaponData;
using Manager;
using UnityEngine;
using Cysharp.Threading.Tasks;
using OutGame;

public class DataManager : MonoBehaviour
{
    private const string itemPath = "Items";

    public Weapon[] weapons;
    public List<ItemData> items = new List<ItemData>();

    public bool isLoaded = false;
    void Init()
    {
        LoadInGameDatas();
    }
    public void LoadData()
    {
        Init();
        foreach (var item in items)
        {
            item.itemDataInfo.curLevel = 0;
            // 새 게임이면 홍길동 단검 레벨 1로 시작
            if (item.itemDataInfo.itemId == WeaponId.Dagger)
            {
                item.itemDataInfo.curLevel = 1;
            }
        }
        foreach (var item in items)
        {
            item.Reset();
        }
        Global.UserDataManager.storage.itemDataInfoList = items.Select(item => item.itemDataInfo).ToList();
        Global.UserDataManager.Save();
    }

    private void Awake()
    {
        items = Resources.LoadAll<ItemData>(itemPath).ToList();
        
        if(Global.UserDataManager.storage.itemDataInfoList.Count == 0)
        {
            LoadData();
        }
        LoadUserData().Forget();

    }

    private async UniTaskVoid LoadUserData()
    {
        await UniTask.WaitUntil(() => Global.UserDataManager != null);

        isLoaded = true;
    }
    private void LoadInGameDatas()
    {
        weapons = GameManager.Instance.weaponController.Weapons.ToArray();
        items = Resources.LoadAll<ItemData>(itemPath).ToList();
    }

    public ItemDataInfo GetItemDataInfo(ItemData itemData)
    {
        ItemDataInfo info = null;
        foreach (var itemDataInfo in Global.UserDataManager.storage.itemDataInfoList)
        {
            if (itemData.itemDataInfo.itemId == itemDataInfo.itemId)
            {
                info = itemDataInfo;
                return info;
            }
        }
        return info;
    }
    public async UniTask<ItemDataInfo> GetItemDataInfo(WeaponId itemId)
    {
        if (!isLoaded)
        {
            await WaitForLoading();
        }
        ItemDataInfo info = null;

        foreach (var itemDataInfo in Global.UserDataManager.storage.itemDataInfoList)
        {
            if (itemId == itemDataInfo.itemId)
            {
                info = itemDataInfo;
                return info;
            }
        }

        if (info == null)
        {
            Debug.LogWarning($"WeaponId {itemId}에 해당하는 ItemDataInfo를 찾을 수 없습니다.");
        }
        return info;
    }
    public ItemData[] GetNotMaxLevelItems()
    {
        List<ItemData> notMaxLevelItems = new List<ItemData>();
        foreach (var item in items)
        {
            var info = GetItemDataInfo(item);
            if (info != null && info.curLevel < info.maxLevel)
            {
                notMaxLevelItems.Add(item);
            }
        }
        return notMaxLevelItems.ToArray();
    }
    public ItemData[] GetMaxLevelItems()
    {
        List<ItemData> maxLevelItems = new List<ItemData>();
        foreach (var item in items)
        {
            var info = GetItemDataInfo(item);
            if (info != null && info.curLevel >= info.maxLevel)
            {
                maxLevelItems.Add(item);
            }
        }
        return maxLevelItems.ToArray();
    }
    public void SetWeaponItemLevel(ItemData itemData, int level)
    {
        foreach (var item in Global.UserDataManager.storage.itemDataInfoList)
        {
            if (item.itemId == itemData.itemDataInfo.itemId)
            {
                item.curLevel = level;
            }
        }
        Global.UserDataManager.Save();
    }

    public void SaveWeaponData(ItemData itemData)
    {
        foreach (var item in Global.UserDataManager.storage.itemDataInfoList)
        {
            if (item.itemId == itemData.itemDataInfo.itemId)
            {
                item.curCoolDown = itemData.itemDataInfo.curCoolDown;
                item.curDuration = itemData.itemDataInfo.curDuration;
                item.curDamage = itemData.itemDataInfo.curDamage;
                item.curCount = itemData.itemDataInfo.curCount;
                item.curRange = itemData.itemDataInfo.curRange;
                item.curCriticalChancePercent = itemData.itemDataInfo.curCriticalChancePercent;
                item.curCriticalDamagePercent = itemData.itemDataInfo.curCriticalDamagePercent;
            }
        }
        Global.UserDataManager.Save();
    }

    public void SaveData()
    {
        LoadInGameDatas();
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
        Global.UserDataManager.Save();
        LoadData();
    }

}