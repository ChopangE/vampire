using System.Collections;
using System.Collections.Generic;
using SO;
using UnityEngine;
using InGame;
using System;

namespace Manager
{
    public class ShopItemInGameManager : MMSingleton<ShopItemInGameManager>
    {
        // 활성화된 아이템 효과 추적
        private Dictionary<string, GameObject> activeItems = new Dictionary<string, GameObject>();
        
        // 아이템 프리팹 참조 (인스펙터에서 설정)
        [SerializeField] private GameObject magnetItemPrefab;
        [SerializeField] private GameObject invincibleItemPrefab;
        [SerializeField] private GameObject masterScrollPrefab;
        
        // 아이템 사용 메서드
        public bool UseActiveItem(ShopItemLevelUpgradeSO shopItem)
        {
            if(shopItem.ItemType == ShopItemType.Active)
            {
                // 아이템 구매 여부 확인 (필요에 따라 제거 가능)
                if(Global.UserDataManager.IsShopItemPurchased(shopItem.Id))
                {
                    // 아이템 사용 처리
                    Global.UserDataManager.UseActiveItem(shopItem.Id);
                    
                    // 아이템 효과 적용
                    ActivateItem(shopItem.Id);
                    return true;
                }
            }
            if(shopItem.ItemType == ShopItemType.Evolution)
            {
                if(Global.UserDataManager.IsShopItemPurchased(shopItem.Id))
                {
                    ActivateItem(shopItem.Id);
                    return true;
                }
            }
            return false;
        }
        
        // 아이템 활성화 메서드
        private void ActivateItem(string itemId)
        {
            // 이미 활성화된 같은 아이템이 있다면 제거
            if (activeItems.ContainsKey(itemId))
            {
                Destroy(activeItems[itemId]);
                activeItems.Remove(itemId);
            }
            
            // 아이템 ID에 따라 적절한 프리팹 생성
            GameObject itemObject = null;
            
            switch (itemId)
            {
                case "ExpMagnetUp":
                    itemObject = CreateItemEffect(magnetItemPrefab);
                    break;
                    
                case "invinciblePotion":
                    itemObject = CreateItemEffect(invincibleItemPrefab);
                    break;
                    
                case "MasterBook":
                    itemObject = CreateItemEffect(masterScrollPrefab);
                    break;
                    
                default:
                    Debug.LogWarning($"정의되지 않은 아이템: {itemId}");
                    return;
            }
            
            // 생성된 아이템 오브젝트 추적
            if (itemObject != null)
            {
                activeItems[itemId] = itemObject;
                
                // 아이템 효과가 종료될 때 호출될 콜백 등록
                IActiveItem activeItem = itemObject.GetComponent<IActiveItem>();
                if (activeItem != null)
                {
                    activeItem.OnEffectEnd += () => {
                        if (activeItems.ContainsKey(itemId))
                        {
                            activeItems.Remove(itemId);
                        }
                    };
                }
            }
        }
        
        // 아이템 효과 오브젝트 생성
        private GameObject CreateItemEffect(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("아이템 프리팹이 설정되지 않았습니다.");
                return null;
            }
            
            // 플레이어 위치에 아이템 효과 생성
            if (GameManager.Instance != null && GameManager.Instance.player != null)
            {
                GameObject itemObject = Instantiate(prefab, GameManager.Instance.player.transform.position, Quaternion.identity);
                
                // 필요에 따라 플레이어에 부착
                IActiveItem activeItem = itemObject.GetComponent<IActiveItem>();
                if (activeItem != null && activeItem.AttachToPlayer)
                {
                    itemObject.transform.parent = GameManager.Instance.player.transform;
                }
                
                return itemObject;
            }
            
            return null;
        }
    }
}

