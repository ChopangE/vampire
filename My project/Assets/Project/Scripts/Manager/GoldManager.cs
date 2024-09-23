using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using System;
using Unity.VisualScripting;

namespace Manager
{
    public class GoldManager
    {
        public void Initial()
        {
            Gold = Global.UserDataManager.GetGoldData();
        }
        public event EventHandler<int> OnGoldValueChanged;
        public event EventHandler<int> OnGoldValueIncreased;
        public event EventHandler<int> OnGoldValueDecreased;
        //* 저장할 땐 string 형식으로 바꿔서 큰 값을 저장
        private int _gold;
        public int Gold
        {
            get{return _gold;}
            private set{
                if (value > _gold) OnGoldValueIncreased?.Invoke(this, value);
                else if (value < _gold) OnGoldValueDecreased?.Invoke(this, value);

                _gold = value;
                OnGoldValueChanged?.Invoke(this, _gold);
            }
        }

        // private string[] goldUnitArr = new string[] {"",
    // "a","b","c","d","e","f","g","h","I","J","K","L","M","N","O","P","q","r","s","t","u","v","w","x","y","z"};
        public void AddGold(int amt)
        {
            Gold += amt;
            Global.UserDataManager.SetGoldData(Gold);
        }
        public void AddGold(string amt)
        {
            Gold += int.Parse(amt);
            Global.UserDataManager.SetGoldData(Gold);
        }
        public void SubGold(int amt)
        {
            Gold -= amt;
            Global.UserDataManager.SetGoldData(Gold);
        }
        public void SubGold(string amt)
        {
            Gold -= int.Parse(amt);
            Global.UserDataManager.SetGoldData(Gold);
        }
        public string GetGoldText()
        {
            return Global.UserDataManager.GetGoldDataString();
        }
        public BigInteger GetGoldValue(){
            return Gold;
        }
        public bool CanPurchase(string cost)
        {
            return GetGoldValue() >= int.Parse(cost);
        }
        
        public bool CanPurchase(BigInteger cost)
        {
            return GetGoldValue() >= cost;
        }
    }
}