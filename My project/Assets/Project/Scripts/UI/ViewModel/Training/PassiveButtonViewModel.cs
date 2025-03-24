using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using I2.Loc;
using Manager;
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
            if(_levelUpgradeSO == null) 
            {
                Debug.LogWarning("PassiveButtonViewModel : DoUpgrade : _levelUpgradeSO is null");
                Global.SoundManager.PlaySFX(SFXEnum.GetMapItem);
                return;
            }
            
            if(_levelUpgradeSO.GetUpgradeLevel() >= _levelUpgradeSO.GetMaxLevel())
            {
                Debug.LogWarning("PassiveButtonViewModel : DoUpgrade : _levelUpgradeSO is max level");
                Global.SoundManager.PlaySFX(SFXEnum.GetMapItem);
                return;
            }

            // 구매 가능한지 확인
            if (!Global.GoldManager.CanPurchase(_levelUpgradeSO.GetUpgradeCost()))
            {
                Debug.LogWarning("PassiveButtonViewModel : DoUpgrade : CanPurchase is false");
                Global.SoundManager.PlaySFX(SFXEnum.GetMapItem);
                return;
            }

            // 골드 차감
            Global.GoldManager.SubGold(_levelUpgradeSO.GetUpgradeCost());

            // 업그레이드 실행
            if (_levelUpgradeSO.DoUpgrade())
            {
                Global.SoundManager.PlaySFX(SFXEnum.Shop_ItemBuy);
            }
            // UI 갱신
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
            if(_levelUpgradeSO.IsMaxLevel())
            {
                PassiveCost = "MAX";
            }
            else
            {
                PassiveCost = _levelUpgradeSO.GetUpgradeCost();
            }
            PassiveInfo = LocalizationManager.GetTranslation(_levelUpgradeSO.descriptionKey);
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