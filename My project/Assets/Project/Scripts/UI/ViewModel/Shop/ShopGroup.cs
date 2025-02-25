using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SO;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class ShopGroup : GroupView
    {
        List<ShopItemLevelUpgradeSO> _levelUpgradeSOList = new List<ShopItemLevelUpgradeSO>();
        public void AddToGroup(ShopItemLevelUpgradeSO levelUpgradeSO)
        {   
            if(!_levelUpgradeSOList.Contains(levelUpgradeSO))
                _levelUpgradeSOList.Add(levelUpgradeSO);
        }

        public void InitialGorup()
        {
            // 패시브 아이템을 앞으로 정렬
            _levelUpgradeSOList = _levelUpgradeSOList
                .OrderBy(item => item.ItemType != ShopItemType.Passive) // false가 먼저 오도록 정렬 (Passive가 먼저)
                .ToList();

            PrepareViewModels(_levelUpgradeSOList.Count);
            var models = GetViewModels();

            //* 패시브를 요소들에게 하나하나 지정
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i] is ShopItemViewModel model)
                {
                    model.SetShopItem(_levelUpgradeSOList[i]);
                }
            }
        }
    }
}