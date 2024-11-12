using System;
using UnityEngine;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class SkillItemViewModel : ViewModel
    {
        private int _skillLevel;
        private float _cooldownProgress;
        private bool _isActive;
        private string _skillName;
        private Sprite _skillIcon;

        [Binding]
        public int SkillLevel
        {
            get => _skillLevel;
            set
            {
                _skillLevel = value;
                OnPropertyChanged(nameof(SkillLevel));
            }
        }

        [Binding]
        public float CooldownProgress
        {
            get => _cooldownProgress;
            set
            {
                _cooldownProgress = value;
                OnPropertyChanged(nameof(CooldownProgress));
            }
        }

        [Binding]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        [Binding]
        public string SkillName
        {
            get => _skillName;
            set
            {
                _skillName = value;
                OnPropertyChanged(nameof(SkillName));
            }
        }

        [Binding]
        public Sprite SkillIcon
        {
            get => _skillIcon;
            set
            {
                _skillIcon = value;
                OnPropertyChanged(nameof(SkillIcon));
            }
        }

        private Weapon _weapon;
        public void Init(Weapon weapon)
        {
            CleanupWeapon();
            
            if (weapon == null)
                return;
                
            SkillIcon = weapon.data.itemIcon;
            SkillLevel = weapon.level;
            _weapon = weapon;
            _weapon.OnSkillCooldownUpdate += UpdateCooldownProgress;
            _weapon.OnSkillLevelUp += OnSkillLevelUp;
        }

        private void CleanupWeapon()
        {
            if (_weapon != null)
            {
                _weapon.OnSkillCooldownUpdate -= UpdateCooldownProgress;
                _weapon.OnSkillLevelUp -= OnSkillLevelUp;
                _weapon = null;
            }
        }

        private void OnDisable()
        {
            CleanupWeapon();
        }

        private void OnDestroy()
        {
            CleanupWeapon();
        }

        private void UpdateCooldownProgress(object sender, float progress)
        {
            CooldownProgress = progress;
        }

        private void OnSkillLevelUp(object sender, int level)
        {
            SkillLevel = level;
        }
    }
}