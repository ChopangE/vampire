using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Data
{
    public struct DamageData
    {
        public float damage;
        public bool isCritical;
        public bool isBonus;

        public DamageData(float damage, bool isCritical, bool isBonus)
        {
            this.damage = damage;
            this.isCritical = isCritical;
            this.isBonus = isBonus;
        }
    }
}