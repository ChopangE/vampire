using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InGame;
using ObjectPooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager.InGame
{
    public class ExpManager : ObjectPoolManager
    {
        public event EventHandler<ExpItem> OnExpSpawned;
        [SerializeField] private Transform ExpGroup;

        private List<ExpItem> expItemList;
        private List<ExpItem> spawnedItemList;
        private List<ObjectPool<PoolObject>> expPool = new List<ObjectPool<PoolObject>>();

        private void Awake() {
            if(ExpGroup == null) ExpGroup = this.transform;
        }
        private void OnEnable() {
            LoadAllExpItems();
            ResetAllPools();
        }
        public void SpawnRandomExpItem(Transform spawnPos)
        {
            int waitingExpItemIndex = Random.Range(0, expItemList.Count);
            SpawnExpItem(waitingExpItemIndex, spawnPos);
        }
        public void SpawnExpItem(int index, Transform spawnPos)
        {
            ExpItem item = expPool[index].PullGameObject(spawnPos.position, Quaternion.identity, ExpGroup)
            .GetComponent<ExpItem>();
        }
        private void LoadAllExpItems()
        {
            expItemList = new List<ExpItem>();
            expItemList = Resources.LoadAll<ExpItem>("Prefabs/ExpItem").ToList();
        }
        protected override void ResetAllPools()
        {
            expPool = new List<ObjectPool<PoolObject>>();
            ObjectPool<PoolObject> o;
            for(int i = 0; i < expItemList.Count; i++) { 
                o = new ObjectPool<PoolObject>(expItemList[i].gameObject, ResetOnPull, ResetOnPush);
                expPool.Add(o);
            }
        }

        protected override void ResetOnPull(PoolObject poolObject)
        {
            ExpItem expItem = poolObject as ExpItem;
            // dropItem.SetDropItemManager(this);

            activeObjCount++; // 활성화된 객체 수 증가
            OnExpSpawned?.Invoke(this, expItem);
            spawnedItemList.Add(expItem);
        }
        protected override void ResetOnPush(PoolObject pushObject)
        {
            activeObjCount--; // 활성화된 객체 수 감소

            spawnedItemList.Remove(pushObject.GetComponent<ExpItem>());
        }
    }
}
