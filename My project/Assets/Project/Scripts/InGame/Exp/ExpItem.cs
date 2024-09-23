using System;
using System.Collections;
using System.Collections.Generic;
using ObjectPooling;
using UnityEngine;

namespace InGame
{
    [RequireComponent(typeof(DropItemTrigger))]
    public abstract class ExpItem : PoolObject
    {
        public int exp = 0;

        protected DropItemTrigger dropItemTrigger;
        protected virtual void Start()
        {
            dropItemTrigger = GetComponent<DropItemTrigger>();
        }
        protected virtual void DestroyItem()
        {
            gameObject.SetActive(false);
        }
    }
}

