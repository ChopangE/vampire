using System;
using System.Collections;
using System.Collections.Generic;
using SO;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class PassiveGroup : GroupView
    {
        List<PlayerPassiveLevelUpgradeSO> _levelUpgradeSOList = new List<PlayerPassiveLevelUpgradeSO>();
        public void AddToGroup(PlayerPassiveLevelUpgradeSO levelUpgradeSO)
        {   
            if(!_levelUpgradeSOList.Contains(levelUpgradeSO))
                _levelUpgradeSOList.Add(levelUpgradeSO);
        }

        public void InitialGorup()
        {
            PrepareViewModels(_levelUpgradeSOList.Count);
            var models = GetViewModels();

            //* 패시브를 요소들에게 하나하나 지정
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i] as PassiveButtonViewModel)
                {
                    var model = models[i] as PassiveButtonViewModel;
                    model.SetPassive(_levelUpgradeSOList[i]);
                }
            }
        }
    }
}