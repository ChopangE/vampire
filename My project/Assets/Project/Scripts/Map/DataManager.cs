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

public class DataManager : MMSingleton<DataManager>
{
    private const string itemPath = "Assets/Project/Data/Items";

    public playerDataList list = new playerDataList();
    public Weapon[] weapons;
    public List<ItemData> items = new List<ItemData>();

    public bool isLoaded = false;
    void Init()
    {
        list.datalist = new playerData[GameManager.Instance.weaponController.Weapons.Count];
        saveDataToJson();
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
        Global.UserDataManager.storage.itemDataInfoList = items.Select(item => item.itemDataInfo).ToList();
        Global.UserDataManager.Save();
    }

    private void OnEnable()
    {
        items = Util.Data.HelperFunctions.GetScriptableObjects<ItemData>(itemPath);
        if (Global.CurrentScene is InGameScene && GameManager.Instance != null)
        {
            if (GameManager.Instance.isNewGame)
            {
                LoadData();
            }
        }
        LoadUserData().Forget();
    }

    private async UniTaskVoid LoadUserData()
    {
        await UniTask.WaitUntil(() => Global.UserDataManager != null);

        foreach (var itemDataInfo in Global.UserDataManager.storage.itemDataInfoList)
        {
            foreach (var item in items)
            {
                if (itemDataInfo.itemId == item.itemDataInfo.itemId)
                {
                    item.itemDataInfo = itemDataInfo;
                }
            }
        }
        isLoaded = true;
    }
    void saveDataToJson()
    {
        Debug.Log("Save!");
        string result = JsonUtility.ToJson(list);
        string path = Path.Combine(Application.dataPath, "playerData.json");
        File.WriteAllText(path, result);
    }
    void loadDataFromJson()
    {
        string path = Path.Combine(Application.dataPath, "playerData.json");
        string jsonData = File.ReadAllText(path);
        list = JsonUtility.FromJson<playerDataList>(jsonData);
    }

    public void SetData(playerData[] datas)
    {
        list.datalist = datas;
        saveDataToJson();
    }

    public playerData[] GetData()
    {
        loadDataFromJson();
        return list.datalist;
    }

    private void LoadInGameDatas()
    {
        weapons = GameManager.Instance.weaponController.Weapons.ToArray();
        items = Util.Data.HelperFunctions.GetScriptableObjects<ItemData>(itemPath);
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

    public void SaveData()
    {
        LoadInGameDatas();
        // for(int i = 0; i < weapons.Length; i++) {
        //     if (weapons[i].gameObject.activeSelf) {
        //         Debug.Log("Setting");
        //         datas[i].isHave = weapons[i].gameObject.activeSelf;
        //         datas[i].damage = weapons[i].damage;
        //         datas[i].count = weapons[i].count;
        //         datas[i].level = GetItemDataInfo(items[i]).curLevel;
        //     }
        // }
        // DataManager.Instance.SetData(datas);

    }

    private async UniTask WaitForLoading()
    {
        while (!isLoaded)
        {
            await UniTask.Yield();
        }
    }
}


[System.Serializable]
public class playerData
{
    public int id;
    public int level;
    public float damage;
    public int count;
    public bool isHave;
}

[System.Serializable]
public class playerDataList
{
    public playerData[] datalist;
}