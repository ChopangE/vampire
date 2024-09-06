using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;

namespace SO
{
    public abstract class LevelUpgradeSO<StatEnum> : UpgradeSO where StatEnum : Enum
    {
        [Header("업그레이드가 적용될 스탯들")]
        [SerializeField]
        public List<LevelStats<StatEnum>> unitsToUpgrade = new List<LevelStats<StatEnum>>();

        public bool isPercentUpgrade = false;

        protected int _curLevel = 0;
        protected int _maxLevel = 0;
        protected string _levelValue = "1";
        protected string _levelCost = "1";
        public virtual void Initialize()
        {
            SetUpgrade();
        }
        public virtual bool IsMaxLevel()
        {
            return _curLevel >= GetMaxLevel();
        }
        public virtual int GetMaxLevel()
        {
            return _maxLevel;
        }
        public virtual string GetUpgradeCost()
        {
            return _levelCost;
        }

        public virtual string GetUpgradeValue()
        {
            return _levelValue;
        }
        public virtual float GetUpgradeValueConvert()
        {
            float output;
            if (float.TryParse(GetUpgradeValue(), out output))
                return output;
            else
                Debug.Log("숫자가 아닙니다.");
            return output;
        }
        public void SetUpgrade()
        {
            foreach (var unitToUpgrade in unitsToUpgrade)
            {
                unitToUpgrade.SetLevelUpgrade(this);
            }
        }
        public override bool DoUpgrade()
        {
            return TryLevelUpgrade();
        }

        private bool TryLevelUpgrade()
        {
            GetUpgradeLevel();
            if(_curLevel == 0 || IsMaxLevel()) return false;
            _curLevel++;
            SetUpgradeLevel();
            return true;
        }
        
        //* 여기부턴 저장 코드를 담고 있기 때문에 레벨 관리는 자율로 구현하시기 바랍니다.
        public virtual int GetUpgradeLevel()
        {
            UpgradeData data = GetUpgradeDataByName(upgradeName);

            if (data != null)
                _curLevel = data.level;
            return _curLevel;
        }
        public virtual void SetUpgradeLevel()
        {
            UpgradeData data = GetUpgradeDataByName(upgradeName);
            if(data != null)
            {
                data.level = _curLevel;
                Global.UserDataManager.SetUpgradeData(data);
            }
        }
        public UpgradeData GetUpgradeDataByName(string upgradeName)
        {
            UpgradeData upgradeData = Global.UserDataManager.GetUpgradeData(upgradeName);

            return upgradeData;
        }
    }
}