using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using ObjectPooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    [RequireComponent(typeof(DropItemBoxTrigger))]
    public abstract class DropItemBox : PoolObject
    {
        [LabelText("드랍가능한 아이템 리스트 설정")]
        [SerializeField] protected DropItemListSO dropItemListSO;
        protected DropItemBoxTrigger dropItemBoxTrigger;
        public DropItemPoolManager dropItemPoolManager;

        protected virtual void Awake()
        {
            dropItemBoxTrigger = GetComponent<DropItemBoxTrigger>();
        }

        private void OnEnable() {
            dropItemBoxTrigger.OnTriggered += OnTriggered;
        }
        protected override void OnDisable() {
            base.OnDisable();
            dropItemBoxTrigger.OnTriggered -= OnTriggered;
        }

        protected virtual void OnTriggered()
        {

        }

        protected virtual void DestroyItem()
        {
            gameObject.SetActive(false);
        }
    }
}

