using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Data;

namespace Manager
{
    public partial class UserDataManager
    {
        private Dictionary<string, bool> _purchasedShopItems = new Dictionary<string, bool>();
        private Dictionary<string, int> _evolutionStages = new Dictionary<string, int>();  // 아이템별 진화 단계

        public bool IsPurchased(string itemId)
        {
            if (!storage.purchasedShopItems.ContainsKey(itemId))
            {
                storage.purchasedShopItems[itemId] = false;
                Save();
            }
            return storage.purchasedShopItems[itemId];
        }

        public void PurchaseItem(string itemId)
        {
            storage.purchasedShopItems[itemId] = true;
            Save();
        }
        public bool IsShopItemPurchased(string itemId)
        {
            if (!storage.purchasedShopItems.ContainsKey(itemId))
            {
                storage.purchasedShopItems[itemId] = false;
                Save();
            }
            return storage.purchasedShopItems[itemId];
        }

        public void UseActiveItem(string itemId)
        {
            storage.purchasedShopItems[itemId] = false;
            Save();
        }

        public void UseEvolutionItem(string itemId)
        {
            storage.purchasedShopItems[itemId] = false;
            storage.evolutionStages[itemId] = 0;
            Save();
        }

        public int GetEvolutionStage(string itemId)
        {
            // 개별 아이템의 진화 단계가 아닌, 해당 아이템이 진화 체인에서 몇 번째 단계인지 반환
            var allShopItems = Global.StatsUpgradeManager.GetAllShopItems();
            var targetItem = allShopItems.FirstOrDefault(x => x.Id == itemId);
            
            if (targetItem == null || !targetItem.IsEvolutionItem || targetItem.FinalEvolution == null)
                return 0;

            // 같은 진화 체인의 모든 아이템 찾기
            var evolutionChain = allShopItems.Where(x => 
                x.IsEvolutionItem && 
                x.FinalEvolution != null && 
                x.FinalEvolution.Id == targetItem.FinalEvolution.Id).ToList();

            // 첫 번째 아이템부터 차례로 추적하여 현재 아이템의 단계 찾기
            var firstItem = evolutionChain.FirstOrDefault(x => 
                !evolutionChain.Any(other => other.NextEvolution == x));

            if (firstItem == null) return 0;

            int stage = 0;
            var current = firstItem;
            while (current != null)
            {
                if (current.Id == itemId)
                    return stage;
                current = current.NextEvolution;
                stage++;
            }

            return 0;
        }

        public void AdvanceEvolution(string itemId)
        {
            // 진화 아이템 구매는 개별 아이템 구매로 처리
            // 기존 진화 단계 시스템은 사용하지 않음
            PurchaseItem(itemId);
        }

        public bool IsFullyEvolved(string itemId)
        {
            var allShopItems = Global.StatsUpgradeManager.GetAllShopItems();
            var targetItem = allShopItems.FirstOrDefault(x => x.Id == itemId);
            
            if (targetItem == null || !targetItem.IsEvolutionItem)
                return false;

            // 최종 진화 아이템인지 확인 (NextEvolution이 null이거나 자기 자신을 가리키는 경우)
            return targetItem.NextEvolution == null || targetItem.NextEvolution.Id == targetItem.Id;
        }

        [Serializable]
        private class SerializableShopData
        {
            public Dictionary<string, bool> purchasedItems;
        }
    }
}