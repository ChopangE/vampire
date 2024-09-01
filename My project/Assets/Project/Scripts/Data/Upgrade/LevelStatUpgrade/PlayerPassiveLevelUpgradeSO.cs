using System.Collections.Generic;
using UnityEngine;
using Data;
using Manager;

namespace SO
{
    [CreateAssetMenu(menuName = "스탯/Upgrades/Level/Player Passive Level Upgrade")]
    public class PlayerPassiveLevelUpgradeSO : LevelUpgradeSO<PlayerPassiveStat>
    {
        public virtual void Initialize(){
            GetMaxLevel();
            GetUpgradeCost();
            GetUpgradeValue();
            SetUpgrade();
        }
        public override int GetMaxLevel()
        {
            return _maxLevel;
        }
        public override string GetUpgradeCost()
        {
            return _levelCost;
        }
        public override string GetUpgradeValue()
        {
            return _levelValue;
        }

        public override int GetUpgradeLevel()
        {
            UpgradeData data = GetUpgradeDataByName(upgradeName);
            if(data != null)
                _curLevel = data.level;
            return _curLevel;
        }
        public override void SetUpgradeLevel()
        {
            UpgradeData data = GetUpgradeDataByName(upgradeName);
            if(data != null)
            {
                data.level = _curLevel;
                Global.UserDataManager.SetUpgradeData(data);
            }
        }
    }
}
