using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace InGame
{
    public class ExpRedRubbyItem : ExpItem
    {
        
        void OnEnable()
        {
            dropItemTrigger.OnTriggered += ExpUp;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            dropItemTrigger.OnTriggered -= ExpUp;
        }
        public void ExpUp()
        {
            GameManager.Instance.GetExp(exp);
            // ReturnToPool() 호출 제거 - OnDisable에서 자동으로 호출됨
            gameObject.SetActive(false);
        }
    }
}

