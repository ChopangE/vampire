using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class TrainingView : GroupView
    {
        [LabelText("한 페이지 그룹 수")]
        [SerializeField] private int GCount = 2;
        [LabelText("한 그룹에 몇개의 요소가 들어갈 것인가")]
        [SerializeField] private int GCountElement = 4;

        private int curPageIndex = 1;
        private void OnEnable()
        {
            InitialTrainingGroup();
            
            // 골드 변경 이벤트 구독
            Global.GoldManager.OnGoldValueChanged += OnGoldValueChanged;
            // 현재 골드 값 초기화
            Coin = int.Parse(Global.GoldManager.GetGoldText());
        }
        
        private void OnDisable()
        {
            // 골드 변경 이벤트 해제
            Global.GoldManager.OnGoldValueChanged -= OnGoldValueChanged;
        }
        
        private void OnGoldValueChanged(object sender, string goldText)
        {
            if (int.TryParse(goldText, out int goldValue))
            {
                Coin = goldValue;
            }
            else
            {
                // 매우 큰 값의 경우 int.MaxValue로 제한
                Coin = int.MaxValue;
            }
        }
        private void InitialTrainingGroup()
        {
            var allPassives = Global.StatsUpgradeManager.GetAllPlayerPassives();
            //* 책 한페이지당 패시브 4 X 2(그룹 수) 개씩 있으니 4개 묶음 수 구하기
            int passiveUnitCount = 
            (allPassives.Count > GCountElement) ? (allPassives.Count / GCountElement) + 1 : allPassives.Count;

            PrepareViewModels(passiveUnitCount);
            var models = GetViewModels();

            //* 패시브를 묶음 수 별로 패시브 하나하나씩 저장
            int passiveCounter = 0;
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i] as PassiveGroup)
                {
                    var modelGroup = models[i] as PassiveGroup;
                    //* 그룹에 담을 갯수만큼만 담고 다음 그룹으로 넘겨주기
                    while (passiveCounter < GCountElement * (i+1) && passiveCounter < allPassives.Count)
                    {
                        var passive = allPassives[passiveCounter];
                        modelGroup.AddToGroup(passive);
                        passiveCounter++;
                    }
                    modelGroup.InitialGorup();
                }
            }

            SetTrainingGroup(0);
        }
        [Binding]
        public void NextTrainingGroup()
        {
            var models = GetViewModels();

            if(models.Count > GCount)
            {
                SetTrainingGroup(curPageIndex + 1);
            }
        }
        [Binding]
        public void ResetTrainingPassive()
        {
            var allPassives = Global.StatsUpgradeManager.GetAllPlayerPassives();
            System.Numerics.BigInteger totalRefund = 0;

            // 각 패시브의 환급 금액을 계산합니다
            foreach(var upgrade in allPassives)
            {
                var currentLevel = upgrade.GetUpgradeLevel();
                if (currentLevel > 0)
                {
                    var refundAmount = upgrade.GetTotalCostUpToCurrentLevel();
                    if (System.Numerics.BigInteger.TryParse(refundAmount, out var refundValue))
                    {
                        totalRefund += refundValue;
                    }
                    
                    Debug.Log($"{upgrade.upgradeNameKey} 레벨 {currentLevel} -> 환급 금액: {refundAmount}");
                }
            }

            // 총 환급 금액을 플레이어에게 지급
            if (totalRefund > 0)
            {
                Global.GoldManager.AddGold(totalRefund);
                Debug.Log($"총 환급 금액: {totalRefund}");
            }

            // 모든 패시브 레벨을 초기화
            foreach(var upgrade in allPassives)
            {
                upgrade.ResetLevel();
            }

            Global.UserDataManager.Save();
            InitialTrainingGroup();
        }
        [Binding]
        public void PrevTrainingGroup()
        {
            var models = GetViewModels();

            if(models.Count > GCount)
            {
                SetTrainingGroup(curPageIndex - 1);
            }
        }


        private void SetTrainingGroup(int pageIndex)
        {
            if(!IsValidPageIndex(pageIndex)) return;
            var models = GetViewModels();  // 그룹 목록을 가져옴
            //* 일단 다 꺼
            for (int i = 0; i < models.Count; i++) {
                models[i].gameObject.SetActive(false);
            }
            int startIndex = pageIndex * GCount;
            int endIndex = startIndex + GCount;

            if (startIndex < models.Count)
            {
                curPageIndex = pageIndex;

                // 시작 인덱스부터 끝 인덱스까지 그룹을 설정
                for (int i = startIndex; i < endIndex && i < models.Count; i++)
                {
                    models[i].gameObject.SetActive(true);
                }
            }
            else
            {
                // 유효하지 않은 페이지 인덱스 처리 (ex. 페이지가 초과될 때)
                Debug.LogWarning("페이지 인덱스가 범위를 벗어났습니다.");
            }
        }

        private bool IsValidPageIndex(int index)
        {
            var models = GetViewModels();
            int totalPages = Mathf.CeilToInt((float)models.Count / GCount);
            bool isLastPage = index >= totalPages;
            bool isFirstPage = index < 0;
            return !isLastPage && !isFirstPage;            
        }

        private int _coin;

        [Binding]
        public int Coin
        {
            get => _coin;
            set
            {
                _coin = value;
                OnPropertyChanged(nameof(Coin));
            }
        }

    }
}