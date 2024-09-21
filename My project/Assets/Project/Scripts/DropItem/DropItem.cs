using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DropItem
{
    public abstract class DropItem : MonoBehaviour
    {
        protected DropItemTrigger dropItemTrigger;
        protected virtual void Start()
        {
            dropItemTrigger = GetComponent<DropItemTrigger>();
        }
    }
}

