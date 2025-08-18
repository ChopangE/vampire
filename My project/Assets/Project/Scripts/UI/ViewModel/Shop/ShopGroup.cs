using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manager;
using SO;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class ShopGroup : GroupView
    {
        private List<ShopItemLevelUpgradeSO> _levelUpgradeSOList = new List<ShopItemLevelUpgradeSO>();
        
        public void AddToGroup(ShopItemLevelUpgradeSO levelUpgradeSO)
        {   
            // 진화형 아이템의 경우, 현재 진화 단계에 맞는 아이템만 추가
            if (levelUpgradeSO.IsEvolutionItem)
            {
                // 이미 같은 계열의 진화 아이템이 있다면 추가하지 않음
                var existingEvolutionItem = _levelUpgradeSOList.FirstOrDefault(x => 
                    x.IsEvolutionItem && GetEvolutionChainId(x) == GetEvolutionChainId(levelUpgradeSO));
                
                if (existingEvolutionItem != null)
                    return;

                // 현재 진화 체인에서 구매 가능한 다음 아이템 찾기
                var nextAvailableItem = FindNextAvailableEvolutionItem(levelUpgradeSO);
                if (nextAvailableItem != null)
                {
                    _levelUpgradeSOList.Add(nextAvailableItem);
                }
            }
            // 일반 아이템은 그대로 추가
            else if (!_levelUpgradeSOList.Contains(levelUpgradeSO))
            {
                _levelUpgradeSOList.Add(levelUpgradeSO);
            }
        }

        // 진화 체인에서 구매 가능한 다음 아이템을 찾는 메서드
        private ShopItemLevelUpgradeSO FindNextAvailableEvolutionItem(ShopItemLevelUpgradeSO startItem)
        {
            var current = startItem;
            int maxIterations = 10; // 무한 루프 방지
            int iterations = 0;
            
            while (current != null && iterations < maxIterations)
            {
                // 현재 아이템이 구매되지 않았다면
                if (!Global.UserDataManager.IsShopItemPurchased(current.Id))
                {
                    // 최종 진화 아이템인 경우 상점에 표시하지 않음
                    if (Global.UserDataManager.IsFullyEvolved(current.Id))
                    {
                        return null;
                    }
                    // 최종 진화가 아니라면 반환
                    return current;
                }
                
                // 이미 구매되었다면 다음 진화 단계로 이동
                current = current.NextEvolution;
                iterations++;
                
                // NextEvolution이 null이면 더 이상 진화할 수 없으므로 종료
                if (current == null)
                {
                    break;
                }
            }
            
            return null;
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

        // 진화 체인의 첫 번째 아이템인지 확인
        private bool IsFirstEvolutionItem(ShopItemLevelUpgradeSO item)
        {
            // FinalEvolution에서 역추적하여 첫 번째 아이템 찾기
            var allShopItems = Global.StatsUpgradeManager.GetAllShopItems();
            var evolutionChain = allShopItems.Where(x => 
                x.IsEvolutionItem && 
                x.FinalEvolution != null && 
                x.FinalEvolution.Id == item.FinalEvolution?.Id).ToList();

            // NextEvolution이 없는 다른 아이템을 찾아서 첫 번째인지 확인
            foreach (var chainItem in evolutionChain)
            {
                bool isReferencedByOthers = evolutionChain.Any(other => other.NextEvolution == chainItem);
                if (!isReferencedByOthers)
                {
                    return chainItem.Id == item.Id;
                }
            }

            return false;
        }

        public void InitialGorup()
        {
            // 패시브 아이템을 앞으로 정렬하되, 진화형 아이템은 현재 단계만 표시
            _levelUpgradeSOList = _levelUpgradeSOList
                .OrderBy(item => item.ItemType != ShopItemType.Passive) // Passive 먼저
                .ThenBy(item => item.IsEvolutionItem) // 진화형 아이템은 나중에
                .ToList();

            PrepareViewModels(_levelUpgradeSOList.Count);
            var models = GetViewModels();

            for (int i = 0; i < models.Count; i++)
            {
                if (models[i] is ShopItemViewModel model)
                {
                    model.SetShopItem(_levelUpgradeSOList[i]);
                    // 이벤트 구독
                    model.OnEvolutionItemPurchased += OnEvolutionItemPurchased;
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

            // 상점 UI 갱신 - 기존 아이템 유지하면서 UI만 갱신
            PrepareViewModels(_levelUpgradeSOList.Count);
            var models = GetViewModels();

            for (int i = 0; i < models.Count; i++)
            {
                if (models[i] is ShopItemViewModel model)
                {
                    model.SetShopItem(_levelUpgradeSOList[i]);
                    model.OnEvolutionItemPurchased += OnEvolutionItemPurchased;
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