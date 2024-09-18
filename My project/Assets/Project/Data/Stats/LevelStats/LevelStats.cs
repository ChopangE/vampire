using System.Collections.Generic;
using UnityEngine;
using System;
using Manager;

namespace SO
{
    public class LevelStats<StatEnum> : Stats<StatEnum> where StatEnum : Enum
    {
        private LevelUpgradeSO<StatEnum> curLevelUpgrade;
        public override float GetStat(StatEnum stat)
        {
            if(curLevelUpgrade == null)
                Debug.Log("스탯 초기화 안됨");

            if (instanceStats.TryGetValue(stat, out var instanceValue))
                return GetUpgradedValue(instanceValue);
            else if (stats.TryGetValue(stat, out float value))
                return GetUpgradedValue(value);
            else
            {
                return 0;
            }
        }
        public void SetLevelUpgrade(LevelUpgradeSO<StatEnum> levelUpgrade)
        {
            curLevelUpgrade = levelUpgrade;
        }
        private float GetUpgradedValue(float baseValue)
        {
            LevelUpgradeSO<StatEnum> upgrade = curLevelUpgrade;
            if (upgrade.isPercentUpgrade)
                baseValue *= (upgrade.GetUpgradeValueConvert() / 100f) + 1f;
            else
                baseValue = upgrade.GetUpgradeValueConvert();

            return baseValue;
        }
    }
}
