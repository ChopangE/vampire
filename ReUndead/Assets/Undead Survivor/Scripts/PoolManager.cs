using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //프리팹을 보관할 변수
    public GameObject[] prefabs;

    //풀 담당을 하는 리스트들
    List<GameObject>[] pools;

    void Awake() {
        pools = new List<GameObject>[prefabs.Length];
        
        for(int i = 0;  i < pools.Length; i++) {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject Get(int index) {

        GameObject select = null;

        foreach(GameObject item in pools[index]) {
            if (!item.activeSelf) {
                select = item;
                select.SetActive(true);
                break;
            }

        }

        if (!select) {

            select = Instantiate(prefabs[index], transform);        //두번째 인자는 poolmanager 자식으로 넣겠다는 뜻.
            pools[index].Add(select);                               //풀에 새로운 object추가.
        }

        return select;
    }
}
