using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine.SceneManagement;
using UnityWeld;
using UnityWeld.Binding;
using UnityEngine;
using SO;

namespace UI
{
    [Binding]
    public class ShopViewModel : GroupView
    {
        private List<ShopItemLevelUpgradeSO> _shopItems;

        private void OnEnable()
        {
            LoadShopItems();
        }

        private void LoadShopItems()
        {
            // Resources 폴더에서 모든 ShopItemLevelUpgradeSO 로드
            _shopItems = Global.StatsUpgradeManager.GetAllShopItems();
            PrepareViewModels(1);

            var models = GetViewModels();
            foreach (var model in models)
            {
                if (model is ShopGroup shopGroup)
                {
                    foreach (var item in _shopItems)
                    {
                        shopGroup.AddToGroup(item);
                    }
                    shopGroup.InitialGorup();
                }
            }
        }

    }
}