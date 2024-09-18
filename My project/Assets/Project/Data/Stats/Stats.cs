using System.Collections.Generic;
using UnityEngine;
using System;
using Data;

//* 여기 오딘 인스펙터 에셋을 사용하였기 때문에 선언하였습니다. 대체법은 블로그에서 찾아주시기 바랍니다. 
using Sirenix.OdinInspector;
using ButtonAttribute = Sirenix.OdinInspector.ButtonAttribute;

namespace SO
{
    public class Stats<StatEnum> : SerializedScriptableObject where StatEnum : Enum
    {
        public Dictionary<StatEnum, float> instanceStats = new Dictionary<StatEnum, float>();
        public Dictionary<StatEnum, float> stats = new Dictionary<StatEnum, float>();
        private List<StatsUpgrade<StatEnum>> appliedUpgrades = new List<StatsUpgrade<StatEnum>>();

        public event Action<Stats<StatEnum>, StatsUpgrade<StatEnum>> upgradeApplied;

        public virtual float GetStat(StatEnum stat)
        {
            if (instanceStats.TryGetValue(stat, out var instanceValue))
                return GetUpgradedValue(stat, instanceValue);
            else if (stats.TryGetValue(stat, out float value))
                return GetUpgradedValue(stat, value);
            else
            {
                return 0;
            }
        }

        public int GetStatAsInt(StatEnum stat)
        {
            return (int)GetStat(stat);
        }

        public void UnlockUpgrade(StatsUpgrade<StatEnum> upgrade)
        {
            if (!appliedUpgrades.Contains(upgrade))
            {
                appliedUpgrades.Add(upgrade);
                upgradeApplied?.Invoke(this, upgrade);
            }
        }

        private float GetUpgradedValue(StatEnum stat, float baseValue)
        {
            foreach (var upgrade in appliedUpgrades)
            {
                if (!upgrade.upgradeToApply.TryGetValue(stat, out float upgradeValue))
                    continue;

                if (upgrade.isPercentUpgrade)
                    baseValue *= (upgradeValue / 100f) + 1f;
                else
                    baseValue += upgradeValue;
            }

            return baseValue;
        }

        [Button]
        public void ResetAppliedUpgrades()
        {
            appliedUpgrades.Clear();
        }
    }
}