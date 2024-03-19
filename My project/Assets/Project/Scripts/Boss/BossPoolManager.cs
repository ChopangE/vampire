using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPoolManager : MonoBehaviour {
    //Variable for prefabs
    public GameObject[] prefabs;

    //Lists For pool
    List<GameObject>[] pools;

    void Awake() {
        pools = new List<GameObject>[prefabs.Length];

        for (int i = 0; i < pools.Length; i++) {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject Get(int index) {

        GameObject select = null;

        foreach (GameObject item in pools[index]) {
            if (!item.activeSelf) {
                select = item;
                select.SetActive(true);
                break;
            }

        }

        if (!select) {

            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
