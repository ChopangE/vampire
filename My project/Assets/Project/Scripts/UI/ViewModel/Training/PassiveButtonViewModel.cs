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
        private PlayerPassiveLevelUpgradeSO _levelUpgradeSO;
        [SerializeField] private LevelStarsGroup _levelStarsGroup;
        private void UpdateLevelGroup()
        {
            _levelStarsGroup.SetStarCount(_levelUpgradeSO.GetMaxLevel(), _levelUpgradeSO.GetUpgradeLevel());
        }

        [Binding]
        public void DoUpgrade()
        {
            _levelUpgradeSO.DoUpgrade();
            RefreshData();
        }
        public void SetPassive(PlayerPassiveLevelUpgradeSO levelUpgradeSO)
        {
            _levelUpgradeSO = levelUpgradeSO;
            RefreshData();
        }
        private void RefreshData()
        {
            Icon = _levelUpgradeSO.icon;
            PassiveName = _levelUpgradeSO.upgradeName;
            PassiveValue = string.Format("value {0}", _levelUpgradeSO.GetUpgradeValue());
            PassiveLevel = string.Format("LV.{0}", _levelUpgradeSO.GetUpgradeLevel());
            UpdateLevelGroup();
        }
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

        private string _passiveName;
        [Binding]
        public string PassiveName
        {
            get => _passiveName;
            set
            {
                _passiveName = value;
                OnPropertyChanged(nameof(PassiveName));
            }
        }
        private string _passiveValue;
        [Binding]
        public string PassiveValue
        {
            get => _passiveValue;
            set
            {
                _passiveValue = value;
                OnPropertyChanged(nameof(PassiveValue));
            }
        }
        private string _passiveLevel;
        [Binding]
        public string PassiveLevel
        {
            get => _passiveLevel;
            set
            {
                _passiveLevel = value;
                OnPropertyChanged(nameof(PassiveLevel));
            }
        }
    }
}