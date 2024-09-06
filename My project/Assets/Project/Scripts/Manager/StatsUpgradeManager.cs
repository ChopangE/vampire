using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using SO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Manager
{
    public class StatsUpgradeManager : MonoBehaviour
    {
        [SerializeField] private List<PlayerPassiveLevelUpgradeSO> playerPassiveStatsUpgradeList;
        private const string playerStatsPath = "Assets/Project/Scripts/Data/Upgrade/PlayerStat";
        private const string playerPassiveStatsPath = "Assets/Project/Scripts/Data/Upgrade/LevelStatUpgrade";

#if UNITY_EDITOR
        [Button("SO 불러오기")]
        public void LoadAssets()
        {
            playerPassiveStatsUpgradeList = HelperFunctions.GetScriptableObjects<PlayerPassiveLevelUpgradeSO>(playerPassiveStatsPath);
        }
#endif
        private void Start()
        {
            var allStatUpgrades = playerPassiveStatsUpgradeList;
            foreach(var upgrade in allStatUpgrades) {
               upgrade.Initialize(); 
            } 
        }

        public List<PlayerPassiveLevelUpgradeSO> GetAllPassives()
        {
            return playerPassiveStatsUpgradeList;
        }
    }
}