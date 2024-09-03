using System;
using System.Collections.Generic;
using Data;

namespace Manager
{
    public partial class UserDataManager
    {
        public UpgradeData GetUpgradeData(string upgradeName)
        {
            if(storage.upgradeDataList.Count == 0) InitialUpgradeData(upgradeName);
            foreach(var u in storage.upgradeDataList) {
                if(u.upgradeName == upgradeName)
                {
                    return u;
                }
            }
            return storage.upgradeDataList[0];
        }
        public void SetUpgradeData(UpgradeData _upgradeData)
        {
            if(storage.upgradeDataList.Contains(_upgradeData))
                storage.upgradeDataList.Remove(GetUpgradeData(_upgradeData.upgradeName));
            storage.upgradeDataList.Add(_upgradeData);
            Save();
        }
        private UpgradeData InitialUpgradeData(string _upgradeName)
        {
            UpgradeData upgradeData = new UpgradeData();
            upgradeData.upgradeName = _upgradeName;
            upgradeData.level = 1;

            SetUpgradeData(upgradeData);
            
            return upgradeData;
        }
    }
}