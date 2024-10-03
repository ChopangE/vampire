using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public float eliteSpawnTime = 30;

    int level;
    float timer;
    private float eliteTimer;
    void Awake() {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        if (!GameManager.Instance.isLive) return;

        timer += Time.deltaTime;
        eliteTimer += Time.deltaTime;
        
        level = GameManager.Instance.level;      //FloorToInt������ int�� ��ȯ/ ceilToint �ø��� int�� ��ȯ

        if(timer > spawnData[level].spawnTime) {
            Spawn();
            timer = 0f;
        }

        if (eliteTimer >= eliteSpawnTime)
        {
            Debug.Log("Spawn");
            eliteTimer = 0f;
            SpawnElite();
        }


            
    }
    void Spawn() {
        GameObject enemy = GameManager.Instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 1���� �˻��ϴ� ���� : GetComponentsInChildern�� 0��° �ε����� �ڱ��ڽ��̴�.
        if (GameManager.Instance.curStage % 4 == 3) {
            enemy.GetComponent<Enemy>().Init(spawnData[Random.Range(GameManager.Instance.curStage-3, GameManager.Instance.curStage)]);
        }
        else {
            enemy.GetComponent<Enemy>().Init(spawnData[GameManager.Instance.curStage]);
        }
    }
    void SpawnElite() {
        GameObject enemy = GameManager.Instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 1���� �˻��ϴ� ���� : GetComponentsInChildern�� 0��° �ε����� �ڱ��ڽ��̴�.
        if (GameManager.Instance.curStage % 4 == 3) {
            enemy.GetComponent<Enemy>().InitElite(spawnData[Random.Range(GameManager.Instance.curStage-3, GameManager.Instance.curStage)]);
        }
        else {
            enemy.GetComponent<Enemy>().InitElite(spawnData[GameManager.Instance.curStage]);
        }
    }
}


[System.Serializable]
public class SpawnData {

    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}
