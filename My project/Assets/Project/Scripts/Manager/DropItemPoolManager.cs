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

        [LabelText("아이템간 스폰 거리")] [Range(0f, 500f)]
        public float minimumDistanceBetweenItems = 20f; //* 소환 제한 범위
        [LabelText("최대 드롭 아이템 수")]
        [SerializeField] private int maxDropItemsCount;
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

        private void Update()
        {
            if (!autoSpawn)
            {
                return;
            }
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                spawnTimer = spawnTimerMax;

                if (activeObjCount < maxDropItemsCount)
                {
                    SpawnRandomDropItem();
                }
            }
        }
        //* 랜덤 드롭 아이템
        public void SpawnRandomDropItem()
        {
            int waitingDropItemIndex = Random.Range(0, dropItemList.Count);
            SpawnDropItem(waitingDropItemIndex);
        }
        public void SpawnDropItem(int index)
        {
            var mapBounds = MapManager.Instance.GetMapBounds();

            //UnityEngine.Random.insideUnitSphere  구 안에 랜덤 스폰

            Vector3 rangePos = GetRandomPosition(mapBounds);
            
            DropItem item = dropItemPool[index].PullGameObject(rangePos, Quaternion.identity, DropItemsGroup)
            .GetComponent<DropItem>();
            spawnedItemList.Add(item);
        }
        Bounds cameraBounds;
        Vector3 GetRandomPosition(Bounds bounds)
        {
            // 카메라의 뷰포트 범위를 스폰 금지 범위로 설정
            Vector3 cameraBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            Vector3 cameraTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

            // 카메라 금지 영역을 Bounds로 정의
            cameraBounds.SetMinMax(cameraBottomLeft, cameraTopRight);

            // 스폰 금지 범위가 맵 경계보다 큰지 확인
            if (cameraBounds.size.x >= bounds.size.x || cameraBounds.size.y >= bounds.size.y)
            {
                Debug.LogError("카메라 금지 범위가 맵 경계보다 큽니다.");
                return Vector3.zero;
            }
            int attempts = 0;
            int maxAttempts = 200; // 최대 시도 횟수

            Vector3 randomPosition;
            do
            {
                // 중심 기준으로 mapBounds.size 범위 내에서 랜덤 위치 생성
                float randomX = Random.Range((-bounds.size.x + 5)/ 2, (bounds.size.x - 5) / 2);
                float randomY = Random.Range((-bounds.size.y + 5) / 2, (bounds.size.y - 5) / 2);

                // 중심 좌표로부터 계산된 랜덤 위치
                randomPosition = bounds.center + new Vector3(randomX, randomY, 0);
                // 무한 루프 방지
                attempts++;
                if (attempts > maxAttempts)
                {
                    Debug.LogWarning("안전한 위치를 찾을 수 없습니다. 아무곳이나 스폰합니다.");
                    break;
                }
            } while (IsBlockSpawnPosition(randomPosition, cameraBounds)); 
            // 특정 범위 안에서는 스폰하면 다시 스폰시키기

            return randomPosition;
        }

        //* 스폰 금지 위치인가
        private bool IsBlockSpawnPosition(Vector3 spawnPos, Bounds cameraBound)
        {
            spawnPos = new Vector3(spawnPos.x, spawnPos.y, cameraBound.center.z);
            if(cameraBound.Contains(spawnPos)) return true;
            
            foreach (var i in spawnedItemList)
            {
                Vector3 spawnedPosition = i.transform.position;
                if (Vector3.Distance(spawnedPosition, spawnPos) < minimumDistanceBetweenItems)
                {
                    return true;
                }
            }
            return false;
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
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(cameraBounds.center, cameraBounds.size);
        }
    }
}