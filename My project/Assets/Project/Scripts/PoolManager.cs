using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PoolManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static PoolManager Instance;

    //Variable for prefabs
    public GameObject[] prefabs;

    //Lists For pool
    [SerializeField]
    private List<GameObject>[] pools;

    private bool isInitialized = false;

    void Awake() {
        Instance = this;
        pools = new List<GameObject>[prefabs.Length];
        
        for(int i = 0; i < pools.Length; i++) {
            pools[i] = new List<GameObject>();
        }
        isInitialized = true;
    }

    public async UniTask<GameObject> GetAsync(int index, CancellationToken cancellationToken = default)
    {
        // PoolManager가 초기화될 때까지 대기
        while (!isInitialized)
        {
            await UniTask.Yield(cancellationToken);
        }

        return Get(index);
    }

    public GameObject Get(int index) {
        // 초기화 되지 않은 경우 null 반환
        if (pools == null || index >= pools.Length) {
            Debug.LogWarning("Pool이 초기화되지 않았거나 잘못된 인덱스입니다.");
            return null;
        }

        GameObject select = null;

        foreach(GameObject item in pools[index]) {
            if (!item.activeSelf) {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (select == null) {
            select = Instantiate(prefabs[index], transform);        
            pools[index].Add(select);                              
        }

        return select;
    }
}
