using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                // 최종 진화 아이템인 경우 진화 체인 완성 여부로 사용 가능 확인
                if(Global.UserDataManager.IsFullyEvolved(shopItem.Id))
                {
                    // 진화 체인이 완성되었는지 확인
                    if(IsEvolutionChainCompleted(shopItem))
                    {
                        // 전체 진화 체인을 사용 처리
                        UseEntireEvolutionChain(shopItem);
                        
                        ActivateItem(shopItem.Id);
                        return true;
                    }
                }
                // 최종 진화가 아닌 경우 구매 여부 확인
                else if(Global.UserDataManager.IsShopItemPurchased(shopItem.Id))
                {
                    ActivateItem(shopItem.Id);
                    return true;
                }
            }
            return false;
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
        
        // 진화 체인 전체를 사용 처리하는 메서드
        private void UseEntireEvolutionChain(ShopItemLevelUpgradeSO evolutionItem)
        {
            var allShopItems = Global.StatsUpgradeManager.GetAllShopItems();
            var evolutionChain = allShopItems.Where(x => 
                x.IsEvolutionItem && 
                x.FinalEvolution != null && 
                x.FinalEvolution.Id == evolutionItem.FinalEvolution?.Id).ToList();

            // 진화 체인의 모든 아이템을 사용 처리
            foreach (var chainItem in evolutionChain)
            {
                if (Global.UserDataManager.IsShopItemPurchased(chainItem.Id))
                {
                    Debug.Log($"진화 체인 아이템 사용 처리: {chainItem.Id}");
                    Global.UserDataManager.UseEvolutionItem(chainItem.Id);
                }
            }
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
                    
                case "SecretBook1":
                case "SecretBook2":
                case "SecretBook3":
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

