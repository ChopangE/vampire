using System.Collections;
using System.Collections.Generic;
using ObjectPooling;
using UnityEngine;

public abstract class ObjectPoolManager : MonoBehaviour
{
    public int activeObjCount = 0; // 현재 활성화된 오브젝트 수
    protected abstract void ResetOnPull(PoolObject poolObject);
    protected abstract void ResetOnPush(PoolObject poolObject);
    protected abstract void ResetAllPools();

    [Header("자동스폰 원할시 아래 설정")]
    public bool autoSpawn = false;
    public float spawnTimer = 4f;
    public float spawnTimerMax = 4f;
}
