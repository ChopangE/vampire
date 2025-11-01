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
            _maxLevel = GetPassiveGroupData().Count - 1;
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
                // Defense의 경우 직접 퍼센트로 표시
                if (name == "Defense")
                {
                    if (float.TryParse(b.value, out float defenseValue))
                    {
                        // 방어력 값을 그대로 퍼센트로 표시 (10 = 10%)
                        _levelValue = defenseValue.ToString("F0");
                        return _levelValue;
                    }
                }
                _levelValue = b.value;
            }
            return _levelValue;
        }

        /// <summary>
        /// 현재 레벨까지 소모된 총 골드를 계산하여 반환합니다.
        /// </summary>
        /// <returns>소모된 총 골드 문자열</returns>
        public string GetTotalCostUpToCurrentLevel()
        {
            var currentLevel = GetUpgradeLevel();
            if (currentLevel <= 0) return "0";

            var groupData = GetPassiveGroupData();
            System.Numerics.BigInteger totalCost = 0;

            // Debug.Log($"[환급 계산] {upgradeNameKey} - 현재 레벨: {currentLevel}");
            
            // 먼저 데이터 구조를 확인해보자
            // Debug.Log($"[환급 계산] 패시브 데이터 구조 확인:");
            // foreach (var data in groupData)
            // {
            //     Debug.Log($"[환급 계산] Level: {data.level}, GoldCost: {data.goldCost}");
            // }

            // 각 업그레이드 단계에서 지불한 비용 계산
            for (int fromLevel = 0; fromLevel < currentLevel; fromLevel++)
            {
                int toLevel = fromLevel + 1;
                
                // 출발 레벨의 GoldCost가 해당 업그레이드 비용
                var levelElement = groupData.Find(x => x.level == fromLevel);
                if (levelElement != null)
                {
                    if (System.Numerics.BigInteger.TryParse(levelElement.goldCost, out var cost))
                    {
                        totalCost += cost;
                        // Debug.Log($"[환급 계산] 레벨 {fromLevel} → {toLevel} 비용: {cost}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[환급 계산] 레벨 {fromLevel} 데이터를 찾을 수 없음");
                }
            }
            // Debug.Log($"[환급 계산] {upgradeNameKey} - 총 환급 금액: {totalCost}");
            return totalCost.ToString();
        }

        /// <summary>
        /// 패시브 레벨을 초기화합니다.
        /// </summary>
        public void ResetLevel()
        {
            _curLevel = 0;
            SetUpgradeLevel();
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
