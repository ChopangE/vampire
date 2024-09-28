using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class DropItemBoxTrigger : DamageObject
    {
        public event Action OnTriggered;

        public override void Dead()
        {
            OnTriggered?.Invoke();
            gameObject.SetActive(false);
        }
    }
}

