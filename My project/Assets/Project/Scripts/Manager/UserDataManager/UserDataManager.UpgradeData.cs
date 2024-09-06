using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;

namespace Manager
{
    public partial class UserDataManager
    {
        public UpgradeData GetUpgradeData(string upgradeName)
        {
            UpgradeData upgradeData = null;
            foreach(var u in storage.upgradeDataList) {
                if(u.upgradeName == upgradeName)
                {
                    upgradeData = u;
                    return upgradeData;
                }
            }
            //* 못찾았으니 초기화
            return InitialUpgradeData(upgradeName);
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