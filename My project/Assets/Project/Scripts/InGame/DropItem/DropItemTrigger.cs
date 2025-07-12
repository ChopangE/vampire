using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;

namespace InGame
{
    public abstract class DropItemTrigger : MonoBehaviour, ITriggerable
    {
        public event Action OnTriggered;
        void OnEnable()
        {
            GetComponent<Collider2D>().enabled = true;
        }

        public virtual void Triggered()
        {
            OnTriggered?.Invoke();
            Global.SoundManager.PlaySFX(SFXEnum.GetExpStone);
        }


        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            Triggered();
        }
    }
}

