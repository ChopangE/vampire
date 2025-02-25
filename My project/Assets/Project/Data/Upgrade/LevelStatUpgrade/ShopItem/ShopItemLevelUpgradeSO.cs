using System.Collections.Generic;
using UnityEngine;
using Data;
using Manager;
using System.IO;

namespace SO
{
    public enum ShopItemType
    {
        Passive,
        Active
    }

    [CreateAssetMenu(menuName = "스탯/Upgrades/Level/Shop Item Level Upgrade")]
    public class ShopItemLevelUpgradeSO : LevelUpgradeSO<ShopItemStat>
    {
        [SerializeField] private int _price;
        [SerializeField] private string _id;
        [SerializeField] private ShopItemType _itemType;

        public int Price => _price;
        public ShopItemType ItemType => _itemType;
        public string Id 
        {
            get
            {
                // ID가 없거나 비어있으면 name 사용
                if (string.IsNullOrEmpty(_id))
                {
                    _id = name;
                }
                return _id;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // 에디터에서 Asset이 생성되거나 수정될 때 ID 자동 생성
            if (string.IsNullOrEmpty(_id))
            {
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    _id = Path.GetFileNameWithoutExtension(assetPath);
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
        }
#endif
    }
}
