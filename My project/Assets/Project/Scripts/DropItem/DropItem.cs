using System;
using System.Collections;
using System.Collections.Generic;
using ObjectPooling;
using UnityEngine;

namespace InGame
{
    public abstract class DropItem : PoolObject
    {
        protected DropItemTrigger dropItemTrigger;
        protected virtual void Start()
        {
            dropItemTrigger = GetComponent<DropItemTrigger>();
        }
    }
}

