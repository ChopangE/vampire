using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;

namespace Manager
{
    public partial class UserDataManager
    {
        private Dictionary<string, bool> _purchasedShopItems = new Dictionary<string, bool>();

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
        [Serializable]
        private class SerializableShopData
        {
            public Dictionary<string, bool> purchasedItems;
        }
    }
}