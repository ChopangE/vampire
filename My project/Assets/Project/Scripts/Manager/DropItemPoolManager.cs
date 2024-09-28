using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InGame;
using ObjectPooling;
using Sirenix.OdinInspector;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Manager
{
    public class DropItemPoolManager : ObjectPoolManager
    {
        [LabelText("드롭 아이템 소환 그룹")]
        public Transform DropItemsGroup;
        public event EventHandler<DropItem> OnDropItemSpawned;

        private List<DropItem> spawnedItemList; //* 소환된 아이템 리스트
        private List<DropItem> dropItemList;
        private List<ObjectPool<PoolObject>> dropItemPool = new List<ObjectPool<PoolObject>>();

        private void OnEnable() {
            spawnedItemList = new List<DropItem>();
            LoadAllDropItems();
            ResetAllPools();
        }

        private void LoadAllDropItems()
        {
            dropItemList = new List<DropItem>();
            dropItemList = Resources.LoadAll<DropItem>("Prefabs/DropItem").ToList();
        }
        public void SpawnDropItem(DropItem dropItem, Vector2 spawnPos)
        {
            for(int i = 0; i < dropItemList.Count; i++) {
                if(dropItemList[i] == dropItem)
                    SpawnDropItem(i, spawnPos);
            }
        }
        public void SpawnDropItem(int index, Vector2 spawnPos)
        {
            //UnityEngine.Random.insideUnitSphere  구 안에 랜덤 스폰
            DropItem item = dropItemPool[index].PullGameObject(spawnPos, Quaternion.identity, DropItemsGroup)
            .GetComponent<DropItem>();
            spawnedItemList.Add(item);
        }

        protected override void ResetOnPull(PoolObject poolObject)
        {
            DropItem dropItem = poolObject as DropItem;
            // dropItem.SetDropItemManager(this);

            activeObjCount++; // 활성화된 객체 수 증가
            OnDropItemSpawned?.Invoke(this, dropItem);
        }
        //* 다시 폴에 넣으실 때 호출될 함수는 push할 때 호출될 액션에 포함할 함수로 만드는게 맞습니다. 
        protected override void ResetOnPush(PoolObject pushObject)
        {
            activeObjCount--; // 활성화된 객체 수 감소
            spawnedItemList.Remove(pushObject.GetComponent<DropItem>());
        }

        protected override void ResetAllPools()
        {
            dropItemPool = new List<ObjectPool<PoolObject>>();
            ObjectPool<PoolObject> o;
            for(int i = 0; i < dropItemList.Count; i++) { 
                o = new ObjectPool<PoolObject>(dropItemList[i].gameObject, ResetOnPull, ResetOnPush);
                dropItemPool.Add(o);
            }
        }
    }
}