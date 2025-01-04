using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Data.WeaponData;
using Manager;
using UnityEngine;

public class DataManager : MMSingleton<DataManager> {
    private const string itemPath = "Assets/Project/Data/Items";

    public playerData[] datas;
    
    public playerDataList list = new playerDataList();
    public Weapon[] weapons;
    public List<ItemData> items = new List<ItemData>();
    void Init() {
        datas = new playerData[GameManager.Instance.weaponController.Weapons.Count];
        for (int i = 0; i < datas.Length; i++) {
            datas[i] = new playerData();
            datas[i].id = i;
            datas[i].level = 0;
            datas[i].damage = 0;
            datas[i].count = 0;
            datas[i].isHave = false;
        }
        list.datalist = datas;
        saveDataToJson();
        LoadInGameDatas();
    }
    
    void saveDataToJson() {
        Debug.Log("Save!");
        string result = JsonUtility.ToJson(list);
        string path = Path.Combine(Application.dataPath, "playerData.json");
        File.WriteAllText(path, result);
    }
    void loadDataFromJson() {
        string path = Path.Combine(Application.dataPath, "playerData.json");
        string jsonData = File.ReadAllText(path);
        list = JsonUtility.FromJson<playerDataList>(jsonData);
    }
    
    public void SetData(playerData[] datas) {
        list.datalist = datas;
        saveDataToJson();
    }
    
    public playerData[] GetData() {
        loadDataFromJson();
        return list.datalist;
    }

    public void LoadData(bool isNewGame = false)
    {
        if(isNewGame) {
            Init();
            Global.UserDataManager.storage.itemDataInfoList = items.Select(item => item.itemDataInfo).ToList();
            Global.UserDataManager.Save();
        }
        if (datas == null || weapons == null || items == null) {
            Debug.LogError("필수 데이터가 초기화되지 않았습니다.");
            return;
        }

        for(int i = 0; i < datas.Length; i++) {
            if (datas[i].isHave && i < weapons.Length) {
                weapons[i].gameObject.SetActive(true);
                weapons[i].damage = datas[i].damage;
                weapons[i].count = datas[i].count;
                items[i].itemDataInfo.curLevel = datas[i].level;
                
                weapons[i].Init();
            }
        }
    }

    private void LoadInGameDatas()
    {
        datas = GetData();
        weapons = GameManager.Instance.weaponController.Weapons.ToArray();
        items = Util.Data.HelperFunctions.GetScriptableObjects<ItemData>(itemPath);
    }

    public ItemDataInfo GetItemDataInfo(ItemData itemData) {
        ItemDataInfo info = null;

        foreach(var itemDataInfo in Global.UserDataManager.storage.itemDataInfoList) {
            if (itemData.itemDataInfo.itemId == itemDataInfo.itemId) {
                info = itemDataInfo;
                return info;
            }
        }
        return info;
    }
    public ItemDataInfo GetItemDataInfo(WeaponId itemId) {
        ItemDataInfo info = null;

        foreach(var itemDataInfo in Global.UserDataManager.storage.itemDataInfoList) {
            if (itemId == itemDataInfo.itemId) {
                info = itemDataInfo;
                return info;
            }
        }

        if(info == null) {
            Debug.LogWarning($"WeaponId {itemId}에 해당하는 ItemDataInfo를 찾을 수 없습니다.");
        }
        return info;
    }
    public ItemData[] GetNotMaxLevelItems() {
        List<ItemData> notMaxLevelItems = new List<ItemData>();
        foreach(var item in items) {
            if(GetItemDataInfo(item).curLevel != GetItemDataInfo(item).maxLevel) {
                notMaxLevelItems.Add(item);
            }
        }
        return notMaxLevelItems.ToArray();
    }

    public void SaveData() {
        LoadInGameDatas();
        for(int i = 0; i < weapons.Length; i++) {
            if (weapons[i].gameObject.activeSelf) {
                Debug.Log("Setting");
                datas[i].isHave = weapons[i].gameObject.activeSelf;
                datas[i].damage = weapons[i].damage;
                datas[i].count = weapons[i].count;
                datas[i].level = GetItemDataInfo(items[i]).curLevel;
            }
        }
        DataManager.Instance.SetData(datas);
    }
}


[System.Serializable]
public class playerData {
    public int id;
    public int level;
    public float damage;
    public int count;
    public bool isHave;
}

[System.Serializable]
public class playerDataList {
    public playerData[] datalist;
}