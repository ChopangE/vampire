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
        [LabelText("한 그룹에 몇개의 요소가 들어갈 것인가")]
        [SerializeField] private int GCount = 4;
        private void OnEnable()
        {
            InitialTrainingGroup();
        }

        private void InitialTrainingGroup()
        {
            var allPassives = Global.StatsUpgradeManager.GetAllPassives();
            //* 책 한페이지당 패시브 4 X 2(그룹 수) 개씩 있으니 4개 묶음 수 구하기
            int passiveUnitCount = 
            (allPassives.Count > GCount) ? (allPassives.Count / GCount) + 1 : allPassives.Count;

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
                    while (passiveCounter < GCount * (i+1) && passiveCounter < allPassives.Count)
                    {
                        var passive = allPassives[passiveCounter];
                        modelGroup.AddToGroup(passive);
                        passiveCounter++;
                    }
                    modelGroup.InitialGorup();
                }
            }
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