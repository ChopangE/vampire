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
            ReturnToPool();
        }
    }
}

