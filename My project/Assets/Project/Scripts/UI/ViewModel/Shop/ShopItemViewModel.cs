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
        
        // 진화 아이템 구매 완료 시 발생하는 이벤트
        public event Action<ShopItemLevelUpgradeSO> OnEvolutionItemPurchased;

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
            // 진화형이 아닌 일반 아이템의 경우에만 구매 여부 체크
            if (!_shopItem.IsEvolutionItem && IsPurchased)
                return;
            
            if (!Global.GoldManager.CanPurchase(_shopItem.Price))
                return;

            Global.GoldManager.SubGold(_shopItem.Price);
            
            if (_shopItem.IsEvolutionItem)
            {
                Global.UserDataManager.AdvanceEvolution(_shopItem.Id);
                
                if (Global.UserDataManager.IsFullyEvolved(_shopItem.Id))
                {
                    // 최종 진화 달성 시 최종 아이템으로 변경
                    Global.UserDataManager.PurchaseItem(_shopItem.FinalEvolution.Id);
                    // 필살기는 인게임에서 확인하여 해금
                }
                
                // 진화 아이템 구매 완료 이벤트 발생
                OnEvolutionItemPurchased?.Invoke(_shopItem);
            }
            else
            {
                Global.UserDataManager.PurchaseItem(_shopItem.Id);
                IsPurchased = true;
            }
            
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