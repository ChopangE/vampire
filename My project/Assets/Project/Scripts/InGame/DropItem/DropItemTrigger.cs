using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public abstract class DropItemTrigger : MonoBehaviour, ITriggerable
    {
        public event Action OnTriggered;

        public virtual void Triggered()
        {
            OnTriggered?.Invoke();
        }


        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            Triggered();
        }
    }
}

