using System;
using System.Numerics;
using UnityEngine;

namespace Manager
{
    public class GoldManager
    {
        public void Initial()
        {
            Gold = Global.UserDataManager.GetGoldDataBigInteger();
        }

        public event EventHandler<string> OnGoldValueChanged;
        public event EventHandler<int> OnGoldValueIncreasedParticle;

        private BigInteger _gold;
        public BigInteger Gold
        {
            get { return _gold; }
            private set
            {
                _gold = value;
                OnGoldValueChanged?.Invoke(this, GetGoldText());
            }
        }

        public void AddGold(BigInteger amt, int spawnParticleCount = 0)
        {
            Gold += amt;
            Global.UserDataManager.SetGoldData(Gold);
            if (spawnParticleCount > 0)
                OnGoldValueIncreasedParticle?.Invoke(this, spawnParticleCount);
        }

        public void AddGold(string amt)
        {
            Gold += BigInteger.Parse(amt);
            Global.UserDataManager.SetGoldData(Gold);
        }

        public void SubGold(BigInteger amt)
        {
            Gold -= amt;
            Global.UserDataManager.SetGoldData(Gold);
        }

        public void SubGold(string amt)
        {
            Gold -= BigInteger.Parse(amt);
            Global.UserDataManager.SetGoldData(Gold);
        }

        public string GetGoldText()
        {
            return Gold.ToString();
        }

        public BigInteger GetGoldValue()
        {
            return Gold;
        }

        public bool CanPurchase(string cost)
        {
            return GetGoldValue() >= BigInteger.Parse(cost);
        }

        public bool CanPurchase(BigInteger cost)
        {
            return GetGoldValue() >= cost;
        }
    }
}