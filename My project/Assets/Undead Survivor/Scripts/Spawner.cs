using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;


    int level;
    float timer;

    void Awake() {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        if (!GameManager.Instance.isLive) return;

        timer += Time.deltaTime;

        level = GameManager.Instance.level;      //FloorToInt������ int�� ��ȯ/ ceilToint �ø��� int�� ��ȯ

        if(timer > spawnData[level].spawnTime) {
            Spawn();
            timer = 0f;
        }


            
    }
    void Spawn() {
        GameObject enemy = GameManager.Instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 1���� �˻��ϴ� ���� : GetComponentsInChildern�� 0��° �ε����� �ڱ��ڽ��̴�.
        enemy.GetComponent<Enemy>().Init(spawnData[Random.Range(0,level + 1)]);
    }
}


[System.Serializable]
public class SpawnData {

    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}
