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
        [SerializeField] private List<LevelUpgradeSO<PlayerStat>> playerStatsUpgradeList;
        [SerializeField] private List<LevelUpgradeSO<PlayerPassiveStat>> playerPassiveStatsUpgradeList;
        private const string playerStatsPath = "Assets/Project/Scripts/Data/Upgrade/PlayerStat";
        private const string playerPassiveStatsPath = "Assets/Project/Scripts/Data/Upgrade/LevelStatUpgrade";

        #if UNITY_EDITOR
        [Button("SO 불러오기")]
        public void LoadAssets()
        {
            playerStatsUpgradeList = HelperFunctions.GetScriptableObjects<LevelUpgradeSO<PlayerStat>>(playerStatsPath);
            playerPassiveStatsUpgradeList = HelperFunctions.GetScriptableObjects<LevelUpgradeSO<PlayerPassiveStat>>(playerPassiveStatsPath);
        }

        public List<LevelUpgradeSO<PlayerPassiveStat>> GetAllPassives()
        {
            return playerPassiveStatsUpgradeList;
        }
#endif
    }
}