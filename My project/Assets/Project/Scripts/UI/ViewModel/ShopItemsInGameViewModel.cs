using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manager;
using SO;
using UnityEngine;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class ShopItemsInGameViewModel : GroupView
    {
        private List<ShopItemLevelUpgradeSO> _levelUpgradeSOList = new List<ShopItemLevelUpgradeSO>();
        private ShopItemLevelUpgradeSO _shopItem;
        
        private void OnEnable() {
            // 구매한 아이템 중 조건에 맞는 아이템만 필터링하여 표시
            FilterPurchasedItems();
            InitialGorup();
        }

        private void FilterPurchasedItems()
        {
            _levelUpgradeSOList.Clear();
            
            // 모든 상점 아이템 중에서 구매한 아이템만 필터링
            var allShopItems = Global.StatsUpgradeManager.GetAllShopItems(); // 모든 상점 아이템 가져오기 (이 메서드는 예시입니다)
            
            foreach (var item in allShopItems)
            {
                bool isPurchased = Global.UserDataManager.IsShopItemPurchased(item.Id);
                
                if (isPurchased)
                {
                    if (item.ItemType == ShopItemType.Evolution)
                    {
                        if (Global.UserDataManager.IsFullyEvolved(item.Id))
                        {
                            if(item.NextEvolution == null)
                                _levelUpgradeSOList.Add(item);
                        }
                    }
                    if(item.ItemType == ShopItemType.Active)
                    {
                        _levelUpgradeSOList.Add(item);
                    }
                    // Passive 타입은 모두 추가
                    // if (item.ItemType == ShopItemType.Passive)
                    // {
                    //     _levelUpgradeSOList.Add(item);
                    // }
                }
            }
        }

        public void AddToGroup(ShopItemLevelUpgradeSO levelUpgradeSO)
        {   
            // 진화형 아이템의 경우, 현재 진화 단계에 맞는 아이템만 추가
            if (levelUpgradeSO.IsEvolutionItem)
            {
                var currentStage = Global.UserDataManager.GetEvolutionStage(levelUpgradeSO.Id);
                
                // 마지막 진화 단계라면 추가하지 않음
                if (Global.UserDataManager.IsFullyEvolved(levelUpgradeSO.Id))
                    return;
                    
                // 이미 같은 계열의 진화 아이템이 있다면 추가하지 않음
                var existingEvolutionItem = _levelUpgradeSOList.FirstOrDefault(x => 
                    x.IsEvolutionItem && x.Id == levelUpgradeSO.Id);
                
                if (existingEvolutionItem != null)
                    return;

                // 첫 단계(stage == 0)이고 첫 번째 아이템인 경우만 추가
                if (currentStage == 0 && levelUpgradeSO.NextEvolution != null)
                {
                    _levelUpgradeSOList.Add(levelUpgradeSO);
                }
                // 현재 단계에 맞는 진화 단계 아이템 추가
                else if (currentStage > 0)
                {
                    var nextEvolution = GetNextEvolutionItem(levelUpgradeSO, currentStage);
                    if (nextEvolution != null && !Global.UserDataManager.IsFullyEvolved(nextEvolution.Id))
                    {
                        _levelUpgradeSOList.Add(nextEvolution);
                    }
                }
            }
            // 일반 아이템은 그대로 추가
            else if (!_levelUpgradeSOList.Contains(levelUpgradeSO))
            {
                _levelUpgradeSOList.Add(levelUpgradeSO);
            }
        }

        private ShopItemLevelUpgradeSO GetNextEvolutionItem(ShopItemLevelUpgradeSO item, int currentStage)
        {
            var current = item;
            for (int i = 0; i < currentStage && current != null; i++)
            {
                current = current.NextEvolution;
            }
            return current;
        }

        public void InitialGorup()
        {
            // 패시브 아이템을 앞으로 정렬하되, 진화형 아이템은 현재 단계만 표시
            _levelUpgradeSOList = _levelUpgradeSOList
                .OrderBy(item => item.ItemType != ShopItemType.Passive) // Passive 먼저
                .ThenBy(item => item.IsEvolutionItem) // 진화형 아이템은 나중에
                .ToList();

            // 최소 3개의 슬롯을 항상 표시
            int slotCount = Math.Max(3, _levelUpgradeSOList.Count);
            PrepareViewModels(slotCount);
            var models = GetViewModels();

            for (int i = 0; i < models.Count; i++)
            {
                if (models[i] is ShopItemInGameSlotViewModel model)
                {
                    if (i < _levelUpgradeSOList.Count)
                    {
                        // 아이템이 있는 경우
                        model.SetShopItem(_levelUpgradeSOList[i], i+1);
                        // 이벤트 구독
                        model.OnEvolutionItemPurchased += OnEvolutionItemPurchased;
                    }
                    else
                    {
                        // 빈 슬롯인 경우
                        model.SetShopItem(null, i+1);
                    }
                }
            }
        }

        private void OnEvolutionItemPurchased(ShopItemLevelUpgradeSO purchasedItem)
        {
            // 구매한 아이템 제거
            _levelUpgradeSOList.Remove(purchasedItem);

            // 아직 최종 진화가 아니라면 다음 단계 아이템 추가
            if (!Global.UserDataManager.IsFullyEvolved(purchasedItem.Id) && purchasedItem.NextEvolution != null)
            {
                // 이미 같은 ID의 다음 단계 아이템이 있는지 확인
                var existingNextEvolution = _levelUpgradeSOList.FirstOrDefault(x => 
                    x.IsEvolutionItem && x.Id == purchasedItem.NextEvolution.Id);
                    
                if (existingNextEvolution == null)
                {
                    _levelUpgradeSOList.Add(purchasedItem.NextEvolution);
                }
            }

            // 상점 UI 갱신 - 최소 3개의 슬롯 유지
            int slotCount = Math.Max(3, _levelUpgradeSOList.Count);
            PrepareViewModels(slotCount);
            var models = GetViewModels();

            for (int i = 0; i < models.Count; i++)
            {
                if (models[i] is ShopItemInGameSlotViewModel model)
                {
                    if (i < _levelUpgradeSOList.Count)
                    {
                        // 아이템이 있는 경우
                        model.SetShopItem(_levelUpgradeSOList[i], i+1);
                        model.OnEvolutionItemPurchased += OnEvolutionItemPurchased;
                    }
                    else
                    {
                        // 빈 슬롯인 경우
                        model.SetShopItem(null, i+1);
                    }
                }
            }
        }

        // 메모리 누수 방지를 위한 정리
        protected void OnDestroy()
        {
            var models = GetViewModels();
            foreach (var model in models)
            {
                if (model is ShopItemViewModel shopModel)
                {
                    shopModel.OnEvolutionItemPurchased -= OnEvolutionItemPurchased;
                }
            }
        }
        private void OnDisable()
        {
            // 리스트 초기화
            _levelUpgradeSOList.Clear();

            // 이벤트 구독 해제 및 뷰모델 초기화
            var models = GetViewModels();
            foreach (var model in models)
            {
                if (model is ShopItemViewModel shopModel)
                {
                    shopModel.OnEvolutionItemPurchased -= OnEvolutionItemPurchased;
                }
            }
            
        }
        
    }
}
