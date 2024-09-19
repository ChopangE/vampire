using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using I2.Loc;
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
            PassiveName = LocalizationManager.GetTranslation(_levelUpgradeSO.upgradeNameKey);
            PassiveValue = _levelUpgradeSO.GetUpgradeValue();
            PassiveLevel = string.Format("LV.{0}", _levelUpgradeSO.GetUpgradeLevel());
            PassiveCost = _levelUpgradeSO.GetUpgradeCost();
            PassiveInfo =  LocalizationManager.GetTranslation(_levelUpgradeSO.descriptionKey);
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
        private string _passiveCost;
        [Binding]
        public string PassiveCost
        {
            get => _passiveCost;
            set
            {
                _passiveCost = value;
                OnPropertyChanged(nameof(PassiveCost));
            }
        }
        private string _passiveInfo;
        [Binding]
        public string PassiveInfo
        {
            get => _passiveInfo;
            set
            {
                _passiveInfo = value;
                OnPropertyChanged(nameof(PassiveInfo));
            }
        }
    }
}