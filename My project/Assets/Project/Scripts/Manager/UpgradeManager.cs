using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Sirenix.OdinInspector;
using SO;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

namespace Manager.InGame
{
    public class DamageUpgradeValues
    {
        public float damagePercent;
        public float critChancePercent;
        public float critDamagePercent;
    }

    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField] private List<SkillUpgradeSO> skillUpgradeSOList;
        private const string skillUpgradePath = "Assets/Project/Data/SkillUpgrade";

#if UNITY_EDITOR
        [Button("SO 불러오기")]
        public void LoadAssets()
        {
            skillUpgradeSOList = HelperFunctions.GetScriptableObjects<SkillUpgradeSO>(skillUpgradePath);
        }
#endif
        public List<SkillUpgradeSO> GetAllSkills()
        {
            return skillUpgradeSOList;
        }

        public SkillUpgradeSO GetRandomSkillUpgrade(List<SkillUpgradeSO> excludeUpgrades = null)
        {
            if (excludeUpgrades == null || excludeUpgrades.Count == 0)
            {
                return skillUpgradeSOList[Random.Range(0, skillUpgradeSOList.Count)];
            }

            var availableUpgrades = skillUpgradeSOList
                .Where(upgrade => !excludeUpgrades.Contains(upgrade))
                .ToList();

            if (availableUpgrades.Count == 0)
            {
                return null;
            }

            return availableUpgrades[Random.Range(0, availableUpgrades.Count)];
        }

        public float GetUpgradeValue(UpgradeName upgradeName)
        {
            switch (upgradeName)
            {
                case UpgradeName.Projectiles:
                    return 1; // 발사체 1개씩 증가

                case UpgradeName.PierceLimit:
                    return 1; // 관통 대상 1명씩 증가

                case UpgradeName.Cooldown:
                    return 0.9f; // 쿨타임 10% 감소 (곱연산용 계수)

                case UpgradeName.Range:
                    return 1.15f; // 범위 15% 증가 (곱연산용 계수)

                case UpgradeName.Duration:
                    return 1.05f; // 지속시간 5% 증가 (곱연산용 계수)

                case UpgradeName.Damage:
                default:
                    Debug.LogError($"Unhandled upgrade type: {upgradeName}");
                    return 0;
            }
        }
        public DamageUpgradeValues GetDamageUpgradeValues()
        {
            // 15~25 사이의 총합을 먼저 결정
            int totalValue = Random.Range(15, 26);
            
            // 첫 번째 값은 남은 값을 고려하여 결정 (최소 5, 최대 10)
            int damagePercent = Mathf.Clamp(Random.Range(5, 11), 5, totalValue - 10);
            
            // 두 번째 값도 남은 값을 고려하여 결정
            int remainingTotal = totalValue - damagePercent;
            int critChancePercent = Mathf.Clamp(Random.Range(5, 11), 5, remainingTotal - 5);
            
            // 마지막 값은 남은 값으로 결정
            int critDamagePercent = totalValue - damagePercent - critChancePercent;
            
            return new DamageUpgradeValues 
            { 
                damagePercent = damagePercent / 100f,
                critChancePercent = critChancePercent / 100f,
                critDamagePercent = critDamagePercent / 100f
            };
        }
    }
}
