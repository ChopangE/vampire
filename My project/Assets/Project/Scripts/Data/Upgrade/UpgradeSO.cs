using Data;
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
        [PropertyOrder(-2)]
        [BoxGroup("디테일")]
        [OdinSerialize]
        [PreviewField(75, ObjectFieldAlignment.Center)]
        [HideLabel]
        public Sprite icon { get; private set; }
        
        [BoxGroup("디테일")]
        [PropertyOrder(-1)]
        public string upgradeName;
        [BoxGroup("디테일")]
        [OdinSerialize]
        [LabelText("설명")]
        public string description { get; private set; }

        [Button(Name = "업그레이드하기")]
        public abstract bool DoUpgrade();
    }
}