using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (!storage.evolutionStages.ContainsKey(itemId))
            {
                storage.evolutionStages[itemId] = 0;
                Save();
            }
            return storage.evolutionStages[itemId];
        }

        public void AdvanceEvolution(string itemId)
        {
            if (!storage.evolutionStages.ContainsKey(itemId))
            {
                storage.evolutionStages[itemId] = 0;
            }
            storage.evolutionStages[itemId]++;
            Save();
        }

        public bool IsFullyEvolved(string itemId)
        {
            return GetEvolutionStage(itemId) >= 3;  // 3단계 진화 완료
        }

        [Serializable]
        private class SerializableShopData
        {
            public Dictionary<string, bool> purchasedItems;
        }
    }
}