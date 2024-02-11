using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //�������� ������ ����
    public GameObject[] prefabs;

    //Ǯ ����� �ϴ� ����Ʈ��
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

            select = Instantiate(prefabs[index], transform);        //�ι�° ���ڴ� poolmanager �ڽ����� �ְڴٴ� ��.
            pools[index].Add(select);                               //Ǯ�� ���ο� object�߰�.
        }

        return select;
    }
}
