using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Data;
using Manager;
using UI.Page;
using UnityWeld;
using UnityWeld.Binding;
using Debug = UnityEngine.Debug;

namespace UI.Page
{
    [Binding]
    public class MapPageNew : PageViewModel
    {
        private string _coin;
        private StageMap stageMap;

        [Binding]
        public string Coin
        {
            get => _coin;
            set
            {
                _coin = value;
                OnPropertyChanged(nameof(Coin));
            }
        }
        private List<GroupView> mapPageViews = new List<GroupView>();
        private void OnEnable()
        {
            var allChildrenPages = GetComponentsInChildren<GroupView>();
            // 자기 자신의 경우엔 무시 
            foreach (var child in allChildrenPages)
            {
                if (child.transform.name != transform.name) mapPageViews.Add(child);
            }

            // StageMap 컴포넌트 찾기
            stageMap = GetComponentInChildren<StageMap>(true);

            OnClickTraingButton();

            // 초기 골드 값 설정
            Coin = Global.GoldManager.GetGoldText();
            // 골드 변경 이벤트 구독
            Global.GoldManager.OnGoldValueChanged += OnGoldValueChanged;
        }
        private void OnDisable()
        {
            // 이벤트 구독 해제
            Global.GoldManager.OnGoldValueChanged -= OnGoldValueChanged;
            foreach (var view in mapPageViews)
            {
                if (view as TrainingView)
                {
                    view.gameObject.SetActive(false);
                }
            }
        }
        private void OnGoldValueChanged(object sender, string newGoldValue)
        {
            Coin = newGoldValue;
        }
        [Binding]
        public void OnClickTraingButton()
        {
            foreach (var view in mapPageViews)
            {
                if (view as TrainingView)
                {
                    view.gameObject.SetActive(true);
                }
                else view.gameObject.SetActive(false);
            }
            if (stageMap != null) stageMap.HideAllStages();
            Global.SoundManager.PlaySFX(SFXEnum.Shop_PageTurn);
        }
        [Binding]
        public void OnClickStageMapPageButton()
        {
            foreach (var view in mapPageViews)
            {
                if (view as StageMapView)
                {
                    view.gameObject.SetActive(true);
                }
                else view.gameObject.SetActive(false);
            }
            if (stageMap != null) stageMap.ShowStage();
            Global.SoundManager.PlaySFX(SFXEnum.Shop_PageTurn);
        }
        [Binding]
        public void OnClickShopPageButton()
        {
            foreach (var view in mapPageViews)
            {
                if (view as ShopViewModel)
                {
                    view.gameObject.SetActive(true);
                }
                else view.gameObject.SetActive(false);
            }
            if (stageMap != null) stageMap.HideAllStages();
            Global.SoundManager.PlaySFX(SFXEnum.Shop_PageTurn);
        }
    }
}