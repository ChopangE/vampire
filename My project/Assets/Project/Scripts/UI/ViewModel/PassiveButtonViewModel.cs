using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using SO;
using UnityEngine;
using UnityWeld;
using UnityWeld.Binding;
namespace UI
{
    [Binding]
    public class PassiveButtonViewModel : ViewModel
    {
        private LevelUpgradeSO<PlayerPassiveStat> _levelUpgradeSO;

        private Sprite _icon;
        [Binding]
        public Sprite Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }


        public void SetPassive(LevelUpgradeSO<PlayerPassiveStat> levelUpgradeSO)
        {
            _levelUpgradeSO = levelUpgradeSO;
            Icon = _levelUpgradeSO.icon;
        }
    }
}