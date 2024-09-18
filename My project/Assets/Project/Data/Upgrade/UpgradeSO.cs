using Data;
using I2.Loc;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using BoxGroupAttribute = Sirenix.OdinInspector.BoxGroupAttribute;
using ButtonAttribute = Sirenix.OdinInspector.ButtonAttribute;

namespace SO
{
    [CreateAssetMenu(menuName = "스탯/Upgrade SO")]
    public abstract class UpgradeSO : SerializedScriptableObject
    {
        [BoxGroup("디테일")] [PropertyOrder(-2)] [OdinSerialize]
        [PreviewField(75, ObjectFieldAlignment.Center)] [HideLabel]
        public Sprite icon { get; private set; }
        
        [BoxGroup("디테일")] [PropertyOrder(-1)]
        public string upgradeName;
        [BoxGroup("디테일")] [PropertyOrder(0)] [TermsPopup("Passive/Name/")]
        public string upgradeNameKey;

        [BoxGroup("디테일")] [PropertyOrder(1)] [OdinSerialize] [LabelText("설명")]
        public string description { get; private set; }
        [BoxGroup("디테일")] [PropertyOrder(2)] [LabelText("설명 번역")] [TermsPopup("Passive/Info/")]
        public string descriptionKey;

        [Button(Name = "업그레이드하기")]
        public abstract bool DoUpgrade();
    }
}