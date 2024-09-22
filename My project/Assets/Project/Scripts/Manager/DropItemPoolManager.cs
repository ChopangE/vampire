using System;
using System.Collections;
using System.Collections.Generic;
using InGame;
using ObjectPooling;
using UnityEngine;


namespace Manager
{
    public class DropItemPoolManager : ObjectPoolManager
    {
        public event EventHandler<DropItem> OnDropItemSpawned;

        //* 스폰 포인트 리스트
        private List<Transform> spawnPointList;
        private void OnEnable() {
            spawnPointList = new List<Transform>();
            ResetAllPools();
        }
        protected override void ResetOnPull(PoolObject poolObject)
        {
            DropItem dropItem = poolObject as DropItem;
            // dropItem.SetDropItemManager(this);

            activeObjCount++; // 활성화된 손님 수 증가
            OnDropItemSpawned?.Invoke(this, dropItem);
        }
        //* 다시 폴에 넣으실 때 호출될 함수는 push할 때 호출될 액션에 포함할 함수로 만드는게 맞습니다. 
        protected override void ResetOnPush(PoolObject pushObject)
        {
            activeObjCount--; // 활성화된 손님 수 감소
        }

        protected override void ResetAllPools()
        {
            
        }
    }
}