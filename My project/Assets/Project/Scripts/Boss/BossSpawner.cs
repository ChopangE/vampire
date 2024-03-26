using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    BossPoolManager poolManager;
    int level;
    float timer;

    void Awake() {
        spawnPoint = GetComponentsInChildren<Transform>();
        poolManager = FindObjectOfType<BossPoolManager>();
    }

    void Update() {
        if (!GameManager.Instance.isLive) return;

        timer += Time.deltaTime;

        if (timer > 1f) {
            Spawn();
            timer = 0f;
        }



    }
    void Spawn() {
        GameObject enemy = poolManager.Get(3);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 1���� �˻��ϴ� ���� : GetComponentsInChildern�� 0��° �ε����� �ڱ��ڽ��̴�.
        enemy.GetComponent<BossEnemy>().Init(Random.Range(0,2));
    }
}

