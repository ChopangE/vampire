using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class SkillMenuViewModel : GroupView
    {
        private List<SkillItemViewModel> _skillItems = new List<SkillItemViewModel>();
        private WeaponController _weaponController;

        private ContentSizeFitter _contentSizeFitter;
        protected override void Awake()
        {
            base.Awake();
            _contentSizeFitter = GetComponent<ContentSizeFitter>();
            _weaponController = FindObjectOfType<WeaponController>();
            _weaponController.OnWeaponActivated += OnWeaponActivated;
            InitializeSkillItems();
        }
        private void InitializeSkillItems()
        {
            if (_weaponController != null)
            {
                PrepareViewModels(_weaponController.Weapons.Count);
            }
            var weapons = _weaponController.Weapons;
            var viewModels = GetViewModels().ToList();
            _skillItems = new List<SkillItemViewModel>();
            foreach(var obj in viewModels) {
                _skillItems.Add(obj as SkillItemViewModel);
            }
            
            for (int i = 0; i < weapons.Count; i++)
            {
                var weapon = weapons[i];
                var skillItem = _skillItems[i];
                
                skillItem.Init(weapon);
            }
            if(_contentSizeFitter != null) {
                RefreshContentSize();
            }
        }
        private void RefreshContentSize()
        {
            System.Collections.IEnumerator Routine()
            {
                var csf = _contentSizeFitter;
                csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                yield return null;
                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            this.StartCoroutine(Routine());
        }
        private void OnWeaponActivated(Weapon weapon)
        {
            InitializeSkillItems();
        }
    }
}