using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace InGame
{
    public class DropCoin : DropItem
    {
        public int coin;
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
        }

        void OnEnable()
        {
            dropItemTrigger.OnTriggered += PlusCoin;

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            dropItemTrigger.OnTriggered -= PlusCoin;
        }
        void PlusCoin()
        {
            int gold = Mathf.RoundToInt(coin * GameManager.Instance.goldBonus);
            Global.GoldManager.AddGold(gold);
            DestroyItem();
        }
    }
}

