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
                var currentStage = Global.UserDataManager.GetEvolutionStage(levelUpgradeSO.Id);
                
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
                    if (nextEvolution != null)
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
                _levelUpgradeSOList.Add(purchasedItem.NextEvolution);
            }

            // 상점 UI 갱신
            InitialGorup();
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