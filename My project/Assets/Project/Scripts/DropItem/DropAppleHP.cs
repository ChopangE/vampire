using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace DropItem
{
    public class DropAppleHP : DropItem
    {
        public float plusHP;
        protected override void Start()
        {
            base.Start();
            dropItemTrigger.OnTriggered += PlusHP;          //Event 구독
        }

        void PlusHP()
        {
            GameManager.Instance.health = Mathf.Min(GameManager.Instance.health += plusHP, GameManager.Instance.maxHealth);
            //임시코드 stat 방식 바뀌면 교체
            Destroy(gameObject);
        }
    }
}

