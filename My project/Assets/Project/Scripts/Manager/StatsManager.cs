using System.Collections.Generic;
using Data;
using SO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Manager
{
    public class StatsManager : MonoBehaviour
    {
        [SerializeField] private List<Stats<PlayerStat>> playerStatsList;
        [SerializeField] private List<Stats<PlayerPassiveStat>> playerPassiveStatsList;
        private const string playerStatsPath = "Assets/Project/Data/Stats/NormalStats";
        private const string playerPassiveStatsPath = "Assets/Project/Data/Stats/LevelStats";

        #if UNITY_EDITOR
        [Button("SO 불러오기")]
        public void LoadAssets()
        {
            playerStatsList = HelperFunctions.GetScriptableObjects<Stats<PlayerStat>>(playerStatsPath);
            playerPassiveStatsList = HelperFunctions.GetScriptableObjects<Stats<PlayerPassiveStat>>(playerPassiveStatsPath);
        }
        #endif
        private void OnApplicationQuit()
        {
            foreach (var stat in playerStatsList)
            {
                stat.ResetAppliedUpgrades();
            }
            foreach (var stat in playerPassiveStatsList)
            {
                stat.ResetAppliedUpgrades();
            }
        }
    }
}