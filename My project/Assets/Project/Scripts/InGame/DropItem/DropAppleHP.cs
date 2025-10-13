using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace InGame
{
    public class DropAppleHP : DropItem
    {
        public float plusHP;
        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            dropItemTrigger.OnTriggered += PlusHP;          //Event 구독
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            dropItemTrigger.OnTriggered -= PlusHP;          //Event 구독
        }
        void PlusHP()
        {
            // plusHP is treated as a percentage of maxHealth (e.g., 20 => 20%)
            float percent = Mathf.Max(0f, plusHP) / 100f; // prevent negative percentages
            float healAmount = GameManager.Instance.maxHealth * percent;

            // Calculate new health without invoking the setter twice
            float newHealth = Mathf.Min(GameManager.Instance.Health + healAmount, GameManager.Instance.maxHealth);
            GameManager.Instance.Health = newHealth;

            // 임시코드 stat 방식 바뀌면 교체
            DestroyItem();
        }

    }
}

