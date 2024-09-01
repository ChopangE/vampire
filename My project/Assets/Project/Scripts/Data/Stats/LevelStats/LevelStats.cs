using System.Collections.Generic;
using UnityEngine;
using System;

namespace SO
{
    public class LevelStats<StatEnum> : Stats<StatEnum> where StatEnum : Enum
    {
        private LevelUpgradeSO<StatEnum> curLevelUpgrade;
        public event Action<Stats<StatEnum>, LevelUpgradeSO<StatEnum>> levelUpgradeApplied;
        public override float GetStat(StatEnum stat)
        {
            if (instanceStats.TryGetValue(stat, out var instanceValue))
                return GetUpgradedValue(instanceValue);
            else if (stats.TryGetValue(stat, out float value))
                return GetUpgradedValue(value);
            else
            {
                return 0;
            }
        }
        private float GetUpgradedValue(float baseValue)
        {
            if(curLevelUpgrade == null) return baseValue;
            LevelUpgradeSO<StatEnum> upgrade = curLevelUpgrade;
            if (upgrade.isPercentUpgrade)
                baseValue *= (upgrade.GetUpgradeValueConvert() / 100f) + 1f;
            else
                baseValue = upgrade.GetUpgradeValueConvert();

            return baseValue;
        }

        public void SetLevelUpgrade(LevelUpgradeSO<StatEnum> levelUpgrade)
        {
            curLevelUpgrade = levelUpgrade;
            levelUpgradeApplied?.Invoke(this, levelUpgrade);
        }
    }
}
