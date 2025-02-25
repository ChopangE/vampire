using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using I2.Loc;
using Manager;
using SO;
using UnityEngine;
using UnityWeld;
using UnityWeld.Binding;
namespace UI
{
    [Binding]
    public class ShopItemViewModel : ViewModel
    {
        private ShopItemLevelUpgradeSO _shopItem;
        private bool _isPurchased;

        [Binding]
        public bool IsPurchased
        {
            get => _isPurchased;
            set
            {
                _isPurchased = value;
                OnPropertyChanged(nameof(IsPurchased));
            }
        }

        [Binding]
        public void DoPurchase()
        {
            if (IsPurchased)
                return;
            
            if (!Global.GoldManager.CanPurchase(_shopItem.Price))
                return;

            Global.GoldManager.SubGold(_shopItem.Price);
            Global.UserDataManager.PurchaseItem(_shopItem.Id);
            IsPurchased = true;
            RefreshData();
        }

        public void SetShopItem(ShopItemLevelUpgradeSO shopItem)
        {
            _shopItem = shopItem;
            IsPurchased = Global.UserDataManager.IsPurchased(_shopItem.Id);
            RefreshData();
        }

        private void RefreshData()
        {
            Icon = _shopItem.icon;
            ShopName = LocalizationManager.GetTranslation(_shopItem.upgradeNameKey);
            ShopValue = _shopItem.GetUpgradeValue();
            ShopLevel = IsPurchased ? "구매완료" : "미구매";
            ShopCost = _shopItem.Price.ToString();
            ShopInfo = LocalizationManager.GetTranslation(_shopItem.descriptionKey);
        }

        private Sprite _icon;
        [Binding]
        public Sprite Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        private string _shopName;
        [Binding]
        public string ShopName
        {
            get => _shopName;
            set
            {
                _shopName = value;
                OnPropertyChanged(nameof(ShopName));
            }
        }
        private string _shopValue;
        [Binding]
        public string ShopValue
        {
            get => _shopValue;
            set
            {
                _shopValue = value;
                OnPropertyChanged(nameof(ShopValue));
            }
        }
        private string _shopLevel;
        [Binding]
        public string ShopLevel
        {
            get => _shopLevel;
            set
            {
                _shopLevel = value;
                OnPropertyChanged(nameof(ShopLevel));
            }
        }
        private string _shopCost;
        [Binding]
        public string ShopCost
        {
            get => _shopCost;
            set
            {
                _shopCost = value;
                OnPropertyChanged(nameof(ShopCost));
            }
        }
        private string _shopInfo;
        [Binding]
        public string ShopInfo
        {
            get => _shopInfo;
            set
            {
                _shopInfo = value;
                OnPropertyChanged(nameof(ShopInfo));
            }
        }
    }
}