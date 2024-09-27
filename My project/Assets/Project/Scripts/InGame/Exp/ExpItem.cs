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
        [SerializeField]
        protected int exp = 0;

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

