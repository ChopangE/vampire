using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 폴링으로 소환하는 예제 스포너입니다.
/// </summary>
namespace ObjectPooling
{
    public class Spawner : MonoBehaviour
    {
        public GameObject cubePrefab;
        public GameObject spherePrefab;
        public GameObject capsulePrefab;
        [Range(1f, 15f)]
        public float range = 5f;
        private static ObjectPool<PoolObject> cubePool;
        private static ObjectPool<PoolObject> spherePool;
        private static ObjectPool<PoolObject> capsulePool;
        public bool canSpawn = true;

        private void OnEnable()
        {
            cubePool = new ObjectPool<PoolObject>(cubePrefab, ResetOnPull, ResetOnPush);
            spherePool = new ObjectPool<PoolObject>(spherePrefab);
            capsulePool = new ObjectPool<PoolObject>(capsulePrefab);

            StartCoroutine(SpawnOverTime());
        }

        IEnumerator SpawnOverTime()
        {
            while (canSpawn)
            {
                Spawn();
                yield return null;
            }
        }

        public void Spawn()
        {
            int random = Random.Range(0, 3);
            Vector3 position = Random.insideUnitSphere * range + this.transform.position;
            GameObject prefab;

            switch (random)
            {
                case 0:
                    prefab = cubePool.PullGameObject(position, Random.rotation);
                    break;
                case 1:
                    prefab = spherePool.PullGameObject(position, Random.rotation);
                    break;
                case 2:
                    prefab = capsulePool.PullGameObject(position, Random.rotation);
                    break;
                default:
                    prefab = cubePool.PullGameObject(position, Random.rotation);
                    break;
            }

            //* 이렇게 스폰시 호출될 구문은 여기말고 아래 호출 함수로 만드세요
            // prefab.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        //* 이렇게 스폰시 호출될 함수는 Pull시 호출될 액션에 포함할 함수로 만드는게 맞습니다. 
        private void ResetOnPull(PoolObject poolObject)
        {
            poolObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        private void ResetOnPush(PoolObject pushObject)
        {
            pushObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}