using System;
using System.Collections;
using System.Collections.Generic;
using ObjectPooling;
using UnityEngine;

namespace InGame
{
    [RequireComponent(typeof(DropItemTrigger))]
    public abstract class DropItem : PoolObject
    {
        protected DropItemTrigger dropItemTrigger;
        protected virtual void Awake()
        {
            dropItemTrigger = GetComponent<DropItemTrigger>();
        }
        protected virtual void DestroyItem()
        {
            gameObject.SetActive(false);
        }
    }
}

