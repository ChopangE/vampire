using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;

namespace InGame
{
    public class ExpRedRubbyTrigger : DropItemTrigger
    {
        protected new event System.Action OnTriggered;
        
        public override void Triggered() 
        {
            OnTriggered?.Invoke();
            Global.SoundManager.PlaySFX(SFXEnum.GetExpStone);
        }
    }
}

