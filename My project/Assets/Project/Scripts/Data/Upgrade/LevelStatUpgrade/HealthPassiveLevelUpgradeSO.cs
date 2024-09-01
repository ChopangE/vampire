using System.Collections;
using System.Collections.Generic;
using Passive;
using SO;
using UnityEngine;


namespace BunnyCafe.SO
{
    [CreateAssetMenu(fileName = "HealthPassiveLevelUpgrade", menuName = "스탯/Upgrades/Level Upgrade/패시브/체력")]
    public class HealthPassiveLevelUpgradeSO : PlayerPassiveLevelUpgradeSO
    {
        public override void Initialize()
        {
            GetMaxLevel();
            GetUpgradeCost();
            GetUpgradeValue();
            SetUpgrade();
        }
        public override int GetMaxLevel()
        {
            _maxLevel = GetMap()?.Count ?? 1;
            return _maxLevel;
        }
        public override string GetUpgradeCost()
        {
            var b = GetValue();
            if(b != null)
            {
                _levelCost = b?.goldCost ?? "1";
            }
            return _levelCost;
        }
        public override string GetUpgradeValue()
        {
            var b = GetValue();
            if(b != null)
            {
                _levelValue = b.value;
            }
            return _levelValue;
        }
        private Dictionary<int, Health> GetMap()
        {
            return Health.HealthMap;
        }

        private Health GetValue()
        {
            var Map = GetMap();
            if (Map != null && Map.TryGetValue(GetUpgradeLevel(), out Health value))
            {
                return value;
            }

            return null;
        }
    }
}