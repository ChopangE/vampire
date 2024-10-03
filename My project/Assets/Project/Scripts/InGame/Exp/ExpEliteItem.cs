using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class ExpEliteItem : ExpItem
    {
        void OnEnable()
        {
            exp = (GameManager.Instance.curStage / 4 + 1) * 10; //에러가능성
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

