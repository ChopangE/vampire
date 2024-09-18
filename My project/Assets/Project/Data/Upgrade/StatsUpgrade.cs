using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace SO
{
    public class StatsUpgrade<StatEnum> : UpgradeSO where StatEnum : Enum
    {
        [Header("업그레이드가 적용한 스탯들")]
        [SerializeField]
        public List<Stats<StatEnum>> unitsToUpgrade = new List<Stats<StatEnum>>();
        public Dictionary<StatEnum, float> upgradeToApply = new Dictionary<StatEnum, float>();
        public bool isPercentUpgrade = false;

        public override bool DoUpgrade()
        {
            foreach (var unitToUpgrade in unitsToUpgrade)
            {
                foreach (var upgrade in upgradeToApply)
                {
                    unitToUpgrade.UnlockUpgrade(this);
                }
            }
            return true;
        }
    }
}