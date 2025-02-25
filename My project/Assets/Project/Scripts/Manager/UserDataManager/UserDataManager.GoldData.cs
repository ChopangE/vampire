using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Manager
{
    public partial class UserDataManager
    {
        public BigInteger GetGoldDataBigInteger()
        {
            if (storage.GoldData != "")
                return BigInteger.Parse(storage.GoldData);
            
            storage.GoldData = "0";
            Save();
            return BigInteger.Zero;
        }

        public string GetGoldDataString()
        {
            if (storage.GoldData != "")
                return storage.GoldData;
            
            storage.GoldData = "0";
            Save();
            return "0";
        }

        public void SetGoldData(string value)
        {
            storage.GoldData = value;
            Save();
        }

        public void SetGoldData(BigInteger value)
        {
            storage.GoldData = value.ToString();
            Save();
        }

        public void AddGoldData(BigInteger value)
        {
            var currentGold = GetGoldDataBigInteger();
            storage.GoldData = (currentGold + value).ToString();
            Save();
        }
    }
}