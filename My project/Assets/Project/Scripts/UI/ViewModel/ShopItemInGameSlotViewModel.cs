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
    public class ShopItemInGameSlotViewModel : ViewModel
    {
        private ShopItemLevelUpgradeSO _shopItem;
        private bool _isPurchased;
        
        // 진화 아이템 구매 완료 시 발생하는 이벤트
        public event Action<ShopItemLevelUpgradeSO> OnEvolutionItemPurchased;

        // 이벤트 핸들러 추가 (슬롯 선택 시 발생)
        public event Action<int> OnSlotSelected;

        public void SetShopItem(ShopItemLevelUpgradeSO shopItem, int slotKey)
        {
            _shopItem = shopItem;
            
            // shopItem이 null인 경우 (빈 슬롯)
            if (_shopItem == null)
            {
                IsPurchased = false;
                RefreshDataForEmptySlot();
            }
            else
            {
                Debug.Log("ShopItemInGameSlotViewModel.SetShopItem: " + _shopItem.upgradeNameKey);
                IsPurchased = true; // shopItem이 있으면 항상 구매된 상태로 처리
                RefreshData();
            }
            
            SlotKey = slotKey;
            
            // InputManager 이벤트 구독
            SubscribeToInputEvents();
        }

        // 빈 슬롯을 위한 데이터 설정 메서드
        private void RefreshDataForEmptySlot()
        {
            Icon = null;
            ShopName = "빈 슬롯";
            ShopValue = "";
            ShopLevel = "";
            ShopCost = "";
            ShopInfo = "";
        }

        // InputManager 이벤트 구독 메서드
        private void SubscribeToInputEvents()
        {
            // 이미 구독 중인 경우 중복 구독 방지
            UnsubscribeFromInputEvents();
            
            // 숫자 키 이벤트 구독
            if (Global.InputManager != null)
            {
                Global.InputManager.OnNumberKeyAction += HandleNumberKeyInput;
            }
        }
        
        // InputManager 이벤트 구독 해제 메서드
        private void UnsubscribeFromInputEvents()
        {
            if (Global.InputManager != null)
            {
                Global.InputManager.OnNumberKeyAction -= HandleNumberKeyInput;
            }
        }
        
        // 숫자 키 입력 처리 메서드
        private void HandleNumberKeyInput(object sender, int numberValue)
        {
            if(GameManager.Instance != null)
            {
                if(GameManager.Instance.isStageClear)
                return;
            }
            // 슬롯 키와 입력된 숫자가 일치하는지 확인 (1, 2, 3 키만 처리)
            if (numberValue >= 1 && numberValue <= 3 && numberValue == SlotKey)
            {
                // Debug.Log($"슬롯 {SlotKey} 선택됨");
                OnSlotSelected?.Invoke(SlotKey);
                SelectSlot();
            }
        }
        
        // 슬롯 선택 처리 메서드
        private void SelectSlot()
        {
            // 빈 슬롯인 경우 아무것도 하지 않음
            if (_shopItem == null)
                return;
                
            // 진화형 아이템 중 최종 진화가 아닌 경우 사용할 수 없음
            if (_shopItem.ItemType == ShopItemType.Evolution && !Global.UserDataManager.IsFullyEvolved(_shopItem.Id))
            {
                Debug.Log("중간 진화 단계 아이템은 사용할 수 없습니다.");
                return;
            }
                
            // 슬롯 선택 시 필요한 로직 구현
            // 예: 아이템 정보 표시, 하이라이트 효과 등
            if(ShopItemInGameManager.Instance.UseActiveItem(_shopItem))
            {
                if(_shopItem.ItemType == ShopItemType.Active)
                    Global.SoundManager.PlaySFX(SFXEnum.Shop_MultiBuy);
                IsPurchased = false;
            }
        }
        
        // ViewModel이 파괴될 때 이벤트 구독 해제
        private void OnDestroy()
        {
            UnsubscribeFromInputEvents();
        }

        private void RefreshData()
        {
            // _shopItem이 null인 경우 빈 슬롯 데이터로 설정
            if (_shopItem == null)
            {
                RefreshDataForEmptySlot();
                return;
            }
            
            Icon = _shopItem.icon;
            ShopName = LocalizationManager.GetTranslation(_shopItem.upgradeNameKey);
            ShopValue = _shopItem.GetUpgradeValue();
            
            // 진화형 아이템 중 최종 진화가 아닌 경우 사용불가로 표시
            if (_shopItem.ItemType == ShopItemType.Evolution && !Global.UserDataManager.IsFullyEvolved(_shopItem.Id))
            {
                ShopLevel = "사용불가";
            }
            else
            {
                ShopLevel = IsPurchased ? "구매완료" : "미구매";
            }
            
            ShopCost = _shopItem.Price.ToString();
            ShopInfo = LocalizationManager.GetTranslation(_shopItem.descriptionKey);
        }
        private int _SlotKey;
        [Binding]
        public int SlotKey
        {
            get => _SlotKey;
            set
            {
                _SlotKey = value;
                OnPropertyChanged(nameof(SlotKey));
            }
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
    }
}