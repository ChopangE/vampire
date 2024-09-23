using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Manager
{
    public partial class UserDataManager
    {
        public int GetGoldData()
        {
            //* 돈 데이터 있으면 그냥 리턴하고
            if(storage.GoldData != "") return int.Parse(storage.GoldData);
            //* 아니면 새로 생성하고
            storage.GoldData = "0";
            Save();
            return 0;
        }
        public string GetGoldDataString()
        {
            //* 돈 데이터 있으면 그냥 리턴하고
            if(storage.GoldData != "") return storage.GoldData;
            //* 아니면 새로 생성하고
            string startGold = "0";
            storage.GoldData = startGold;
            Save();
            return startGold;
        }
        public void SetGoldData(string value)
        {
            storage.GoldData = value;
            Save();
        }
        public void SetGoldData(int value)
        {
            storage.GoldData = value.ToString();
            Save();
        }
    }
}