using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MMSingleton<DataManager> {

    private static DataManager instance;
    public playerData[] datas = new playerData[8];
    public playerDataList list = new playerDataList();

    void Start() {
        Init();
    }

    void Init() {
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