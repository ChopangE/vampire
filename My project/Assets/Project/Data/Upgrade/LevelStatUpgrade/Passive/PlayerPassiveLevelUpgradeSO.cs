using System.Collections.Generic;
using UnityEngine;
using Data;
using Manager;

namespace SO
{
    [CreateAssetMenu(menuName = "스탯/Upgrades/Level/Player Passive Level Upgrade")]
    public class PlayerPassiveLevelUpgradeSO : LevelUpgradeSO<PlayerPassiveStat>
    {
        private List<Passive.PlayerStat> playerStatList = new List<Passive.PlayerStat>();
        public override void Initialize(){
            GetPassiveGroupData();
            GetMaxLevel();
            GetUpgradeCost();
            GetUpgradeValue();
            SetUpgrade();
        }
        public override int GetMaxLevel()
        {
            //* 그룹이 있으면 Count 가져오고 아니면 1 리턴
            _maxLevel = GetPassiveGroupData().Count;
            return _maxLevel;
        }
        public override string GetUpgradeCost()
        {
            var b = GetLevelElement();
            if(b != null)
            {
                _levelCost = b?.goldCost ?? "1";
            }
            return _levelCost;
        }
        public override string GetUpgradeValue()
        {
            var b = GetLevelElement();
            if(b != null)
            {
                _levelValue = b.value;
            }
            return _levelValue;
        }

        private Passive.PlayerStat GetLevelElement()
        {
            Passive.PlayerStat value = null;
            foreach (var element in GetPassiveGroupData())
            {
                if (GetUpgradeLevel() == element.level)
                {
                    value = element;
                    return value;
                }
            }
            return value;
        }
        private List<Passive.PlayerStat> GetPassiveGroupData()
        {
            if(playerStatList.Count != 0) return playerStatList;
            playerStatList = new List<Passive.PlayerStat>();
            foreach(var element in Passive.PlayerStat.PlayerStatList) {
                foreach(var unit in unitsToUpgrade) {
                    foreach (var enumValue in unit.stats.Keys) 
                    {
                        if ((int)enumValue == element.GroupID)
                        {
                            playerStatList.Add(element);
                        }
                    }
                }
            }
            if(playerStatList.Count == 0) Debug.LogWarning("스탯 렙업 SO 찾기 실패");
            return playerStatList;
        }
    }
}
