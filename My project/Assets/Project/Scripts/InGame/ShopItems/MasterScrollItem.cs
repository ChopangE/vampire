using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manager;
using SO;
using UI;
using UI.Page;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;

namespace InGame
{
    public class MasterScrollItem : ActiveItemBase
    {
        [Header("마스터북 아이템")]
        [SerializeField] private ShopItemLevelUpgradeSO shopItem;

        [Header("마스터북 성공했을 때 무기")]
        [SerializeField] private GameObject masterWeapon;
        
        // 패턴 성공 여부를 추적하기 위한 변수
        private bool isPatternSucceeded = false;
        // 영상 재생 완료 여부를 추적하기 위한 변수
        private bool isVideoCompleted = false;
        // 마스터 무기 활성화 여부
        private bool isMasterWeaponActive = false;
        private Player player;
        protected override void Start()
        {
            base.Start();
            player = GameManager.Instance?.player;
        }
        
        public override void ActivateEffect()
        {
            // 마스터 무기가 있으면 초기에 비활성화
            if (masterWeapon != null)
            {
                Debug.Log("마스터 무기 비활성화");
                masterWeapon.SetActive(false);
                isMasterWeaponActive = false;
            }
            
            if (player == null)
            {
                player = GameManager.Instance?.player;
                if (player == null)
                {
                    Debug.LogWarning("플레이어 참조를 찾을 수 없습니다.");
                    EndEffect();
                    return;
                }
            }
            
            // 아재 패턴 페이지 열기
            var page = Global.UIManager.OpenPage<MasterBookPage>();
            
            // 페이지의 AzePatternViewModel 찾기
            if (page != null)
            {
                var azePatternViewModel = page.GetComponentInChildren<AzePatternViewModel>();
                if (azePatternViewModel != null)
                {
                    // 패턴 성공 이벤트 구독
                    SubscribeToPatternEvents(azePatternViewModel);
                    
                    // 비디오 완료 이벤트 구독
                    var videoViewModel = azePatternViewModel.GetComponentInChildren<AzePatternVideoViewModel>();
                    if (videoViewModel != null)
                    {
                        SubscribeToVideoEvents(videoViewModel);
                    }
                }
            }
        }
        
        // 패턴 이벤트 구독
        private void SubscribeToPatternEvents(AzePatternViewModel viewModel)
        {
            // 기존 구독 해제
            UnsubscribeFromPatternEvents(viewModel);
            
            // 패턴 성공 이벤트 구독
            viewModel.OnPatternSucceeded += HandlePatternSucceeded;
            viewModel.OnPatternFailed += HandlePatternFailed;
        }
        
        // 패턴 이벤트 구독 해제
        private void UnsubscribeFromPatternEvents(AzePatternViewModel viewModel)
        {
            if (viewModel != null)
            {
                viewModel.OnPatternSucceeded -= HandlePatternSucceeded;
                viewModel.OnPatternFailed -= HandlePatternFailed;
            }
        }
        
        // 비디오 이벤트 구독
        private void SubscribeToVideoEvents(AzePatternVideoViewModel viewModel)
        {
            // 기존 구독 해제
            UnsubscribeFromVideoEvents(viewModel);
            
            // 비디오 완료 이벤트 구독
            viewModel.OnVideoCompleted += HandleVideoCompleted;
        }
        
        // 비디오 이벤트 구독 해제
        private void UnsubscribeFromVideoEvents(AzePatternVideoViewModel viewModel)
        {
            if (viewModel != null)
            {
                viewModel.OnVideoCompleted -= HandleVideoCompleted;
            }
        }
        
        // 패턴 성공 처리
        private void HandlePatternSucceeded()
        {
            isPatternSucceeded = true;
            Debug.Log("마스터북 패턴 성공 - 아이템 사용 처리됨");
            
            // 현재 아이템이 진화형인 경우, 전체 진화 체인을 사용 처리
            if (shopItem != null && shopItem.IsEvolutionItem)
            {
                // ShopItemInGameManager를 통해 진화 체인 전체 사용 처리
                // (ShopItemInGameManager.UseActiveItem에서 UseEntireEvolutionChain 호출)
                if (Global.UserDataManager.IsFullyEvolved(shopItem.Id))
                {
                    UseEntireEvolutionChain(shopItem);
                }
                else
                {
                    Global.UserDataManager.UseEvolutionItem(shopItem.Id);
                }
            }
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
        
        // 패턴 실패 처리
        private void HandlePatternFailed()
        {
            isPatternSucceeded = false;
            Debug.Log("마스터북 패턴 실패 - 아이템 사용 처리되지 않음");
            
            // 패턴 실패 시 효과 종료
            EndEffect();
        }
        
        // 비디오 완료 처리
        private void HandleVideoCompleted()
        {
            isVideoCompleted = true;
            Debug.Log("마스터북 비디오 재생 완료");
            
            // 패턴 성공 및 비디오 완료 시 마스터 무기 활성화
            if (isPatternSucceeded && masterWeapon != null)
            {
                Debug.Log("마스터 무기 활성화");
                ActivateMasterWeapon();
            }
            else
            {
                // 패턴 실패 또는 마스터 무기가 없는 경우 효과 종료
                EndEffect();
            }
        }
        
        // 마스터 무기 활성화
        private void ActivateMasterWeapon()
        {
            if (masterWeapon != null)
            {
                masterWeapon.SetActive(true);
                isMasterWeaponActive = true;
                
                // 필요한 경우 추가 효과 적용
                ApplyMasterWeaponEffects();
                
                // 타이머 없이 계속 활성화 상태 유지
                // 게임 종료나 씬 전환 등 다른 이벤트에 의해서만 비활성화됨
            }
        }
        
        // 마스터 무기 효과 적용 (필요시 구현)
        private void ApplyMasterWeaponEffects()
        {
            // 마스터 무기 활성화 시 추가 효과 적용
            // 예: 파티클 효과, 사운드 재생 등
        }
        
        protected override void EndEffect()
        {
            // 마스터 무기는 비활성화하지 않음 (계속 활성화 상태 유지)
            
            // 효과 종료 이벤트 발생 및 오브젝트 제거
            base.EndEffect();
        }
        
        private void OnDestroy()
        {
            // 페이지의 AzePatternViewModel 찾아서 이벤트 구독 해제
            var page = FindObjectOfType<MasterBookPage>();
            if (page != null)
            {
                var azePatternViewModel = page.GetComponentInChildren<AzePatternViewModel>();
                if (azePatternViewModel != null)
                {
                    UnsubscribeFromPatternEvents(azePatternViewModel);
                    
                    var videoViewModel = azePatternViewModel.GetComponent<AzePatternVideoViewModel>();
                    if (videoViewModel != null)
                    {
                        UnsubscribeFromVideoEvents(videoViewModel);
                    }
                }
            }
        }
    }
}
