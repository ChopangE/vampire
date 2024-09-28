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
    public class DropItemBoxPoolManager : ObjectPoolManager
    {
        public event EventHandler<DropItemBox> OnDropItemBoxSpawned;


        [BoxGroup("세부 설정")] [LabelText("소환할 드롭 아이템 박스")] [SerializeField] 
        private List<GameObject> dropItemBoxList;
        [BoxGroup("세부 설정")] [LabelText("드롭 아이템 박스 소환 그룹")]
        public Transform DropItemsBoxGroup;
        
        [BoxGroup("세부 설정")] [LabelText("아이템간 스폰 거리")] [Range(0f, 500f)]
        public float minimumDistanceBetweenItems = 20f; 
        [BoxGroup("세부 설정")] [LabelText("시작할 때 스폰할 수")] [SerializeField] 
        public int startSpawnCount = 20;
        [BoxGroup("세부 설정")] [LabelText("최대 드롭 아이템 수")] [SerializeField] 
        private int maxDropItemsCount;

        private List<ObjectPool<PoolObject>> dropItemPool = new List<ObjectPool<PoolObject>>();
        private List<DropItemBox> spawnedItemBoxList; //* 소환된 아이템 박스 리스트
        private DropItemPoolManager dropItemPoolManager;
        private void Awake() {
            dropItemPoolManager = FindObjectOfType<DropItemPoolManager>();
        }

        private void OnEnable() {
            spawnedItemBoxList = new List<DropItemBox>();
            ResetAllPools();
        }

        private void Start() {
            for(int i = 0; i < startSpawnCount; i++) {
                SpawnDropItemBox(Random.Range(0, dropItemBoxList.Count));
            }
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
                    SpawnDropItemBox(Random.Range(0, dropItemBoxList.Count));
                }
            }
        }
        
        public void SpawnDropItemBox(int index)
        {
            List<PoolObject> poolObjectList = spawnedItemBoxList.Cast<PoolObject>().ToList();

            Vector3 rangePos = MapManager.Instance.GetRandomSpawnPosition(poolObjectList, minimumDistanceBetweenItems);
            
            DropItemBox item = dropItemPool[index].PullGameObject(rangePos, Quaternion.identity, DropItemsBoxGroup)
            .GetComponent<DropItemBox>();
            spawnedItemBoxList.Add(item);
        }

        protected override void ResetOnPull(PoolObject poolObject)
        {
            DropItemBox dropItemBox = poolObject as DropItemBox;
            dropItemBox.dropItemPoolManager = dropItemPoolManager;
            // dropItem.SetDropItemManager(this);

            activeObjCount++; // 활성화된 객체 수 증가
            OnDropItemBoxSpawned?.Invoke(this, dropItemBox);
        }
        //* 다시 폴에 넣으실 때 호출될 함수는 push할 때 호출될 액션에 포함할 함수로 만드는게 맞습니다. 
        protected override void ResetOnPush(PoolObject pushObject)
        {
            activeObjCount--; // 활성화된 객체 수 감소
            spawnedItemBoxList.Remove(pushObject.GetComponent<DropItemBox>());
        }

        protected override void ResetAllPools()
        {
            dropItemPool = new List<ObjectPool<PoolObject>>();
            ObjectPool<PoolObject> o;
            for(int i = 0; i < dropItemBoxList.Count; i++) { 
                o = new ObjectPool<PoolObject>(dropItemBoxList[i], ResetOnPull, ResetOnPush);
                dropItemPool.Add(o);
            }
        }
    }
}
