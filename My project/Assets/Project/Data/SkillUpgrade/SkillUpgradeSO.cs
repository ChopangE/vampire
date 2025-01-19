using Data;
using I2.Loc;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using BoxGroupAttribute = Sirenix.OdinInspector.BoxGroupAttribute;
using ButtonAttribute = Sirenix.OdinInspector.ButtonAttribute;

namespace SO
{
    public enum UpgradeName {
        Damage, // 데미지
        Projectiles, // 투사체 개수 
        PierceLimit, // 관통 횟수
        Cooldown, // 쿨타임
        Range, // 범위/사거리/크기
        Duration, // 지속 시간
    }
    [CreateAssetMenu(menuName = "업그레이드/Skill Upgrade SO")]
    public class SkillUpgradeSO : SerializedScriptableObject
    {
        [BoxGroup("디테일")] [PropertyOrder(-1)] [LabelText("강화 효과 이름")]
        public UpgradeName upgradeName;
        [BoxGroup("디테일")] [PropertyOrder(0)] [TermsPopup("Skill/Name/")]
        public string upgradeNameKey;
        [BoxGroup("디테일")] [PropertyOrder(1)] [Button("자동 번역키 설정")]
        public void SetUpgradeNameKey() {
            upgradeNameKey = "Skill/Name/" + upgradeName.ToString();
        }
        [BoxGroup("디테일")] [PropertyOrder(2)] [LabelText("강화 효과 정도")]
        public float upgradeValue;
    }
}