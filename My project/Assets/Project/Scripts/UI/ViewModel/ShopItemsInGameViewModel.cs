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
            var allShopItems = Global.StatsUpgradeManager.GetAllShopItems();
            var addedEvolutionChains = new HashSet<string>(); // 이미 추가된 진화 체인 추적
            
            foreach (var item in allShopItems)
            {
                bool isPurchased = Global.UserDataManager.IsShopItemPurchased(item.Id);
                if (isPurchased)
                {
                    // 진화형 아이템: 진화 체인 완성 시 최종 진화 아이템 자동 표시
                    if (item.ItemType == ShopItemType.Evolution)
                    {
                        string chainId = GetEvolutionChainId(item);
                        
                        // 이미 이 진화 체인의 아이템이 추가되었다면 스킵
                        if (addedEvolutionChains.Contains(chainId))
                            continue;
                            
                        // 진화 체인이 완성되었는지 확인 (최종 단계 전까지 모두 구매됨)
                        if (IsEvolutionChainCompleted(item) && item.FinalEvolution != null)
                        {
                            // 최종 진화 아이템을 자동으로 추가 (구매하지 않아도 사용 가능)
                            _levelUpgradeSOList.Add(item.FinalEvolution);
                            addedEvolutionChains.Add(chainId);
                        }
                        // 최종 진화 아이템이 직접 구매된 경우도 처리 (예외적 케이스)
                        else if (Global.UserDataManager.IsFullyEvolved(item.Id) && 
                                Global.UserDataManager.IsShopItemPurchased(item.Id))
                        {
                            _levelUpgradeSOList.Add(item);
                            addedEvolutionChains.Add(chainId);
                        }
                    }
                    // 액티브 아이템: 그대로 추가
                    else if (item.ItemType == ShopItemType.Active)
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

        // 진화 체인의 고유 ID를 반환 (최종 진화 아이템의 ID 사용)
        private string GetEvolutionChainId(ShopItemLevelUpgradeSO item)
        {
            if (item.FinalEvolution != null)
                return item.FinalEvolution.Id;
            return item.Id;
        }

        // 해당 진화 체인에서 소유중인 가장 높은 단계의 아이템을 반환
        private ShopItemLevelUpgradeSO GetHighestOwnedEvolutionItem(ShopItemLevelUpgradeSO item)
        {
            var allShopItems = Global.StatsUpgradeManager.GetAllShopItems();
            var evolutionChain = allShopItems.Where(x => 
                x.IsEvolutionItem && 
                x.FinalEvolution != null && 
                x.FinalEvolution.Id == item.FinalEvolution?.Id).ToList();

            // 역순으로 확인하여 가장 높은 단계의 소유 아이템 찾기
            ShopItemLevelUpgradeSO highestOwned = null;
            foreach (var chainItem in evolutionChain.OrderByDescending(x => GetEvolutionStage(x)))
            {
                if (Global.UserDataManager.IsShopItemPurchased(chainItem.Id))
                {
                    highestOwned = chainItem;
                    break;
                }
            }

            return highestOwned;
        }

        // 아이템의 진화 단계를 계산 (체인에서의 위치)
        private int GetEvolutionStage(ShopItemLevelUpgradeSO item)
        {
            var allShopItems = Global.StatsUpgradeManager.GetAllShopItems();
            var evolutionChain = allShopItems.Where(x => 
                x.IsEvolutionItem && 
                x.FinalEvolution != null && 
                x.FinalEvolution.Id == item.FinalEvolution?.Id).ToList();

            // 첫 번째 아이템부터 차례로 추적
            var firstItem = evolutionChain.FirstOrDefault(x => 
                !evolutionChain.Any(other => other.NextEvolution == x));

            if (firstItem == null) return 0;

            int stage = 0;
            var current = firstItem;
            while (current != null)
            {
                if (current.Id == item.Id)
                    return stage;
                current = current.NextEvolution;
                stage++;
            }

            return 0;
        }

        // 진화 체인이 완성되었는지 확인 (최종 단계 전까지 모두 구매됨)
        private bool IsEvolutionChainCompleted(ShopItemLevelUpgradeSO item)
        {
            var allShopItems = Global.StatsUpgradeManager.GetAllShopItems();
            var evolutionChain = allShopItems.Where(x => 
                x.IsEvolutionItem && 
                x.FinalEvolution != null && 
                x.FinalEvolution.Id == item.FinalEvolution?.Id).ToList();

            // 최종 진화 아이템을 제외한 모든 아이템이 구매되었는지 확인
            foreach (var chainItem in evolutionChain)
            {
                // 최종 진화 아이템이 아닌 경우
                if (!Global.UserDataManager.IsFullyEvolved(chainItem.Id))
                {
                    // 구매되지 않았다면 체인 완성되지 않음
                    if (!Global.UserDataManager.IsShopItemPurchased(chainItem.Id))
                        return false;
                }
            }

            return true;
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
            // 구매한 아이템은 유지하고, 리스트를 새로 필터링
            FilterPurchasedItems();

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
