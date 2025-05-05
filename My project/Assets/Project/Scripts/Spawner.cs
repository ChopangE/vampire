using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] middleBossList;
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
        
        level = GameManager.Instance.CurStage;      
        if(level > spawnData.Length || GameManager.Instance.CurStage == Global.StageManager.MAX_STAGE_COUNT * Global.StageManager.MAX_STAGE_LEVEL) return;
        if(timer > spawnData[level].spawnTime) {
            Spawn();
            timer = 0f;
        }

        if (eliteTimer >= eliteSpawnTime)
        {
            eliteTimer = 0f;
            SpawnElite();
        }


            
    }
    void Spawn() {
        GameObject enemy = GameManager.Instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 1 ˻ϴ  : GetComponentsInChildern 0°ε ڱڽ̴.
        if (GameManager.Instance.CurStage % 4 == 3) {
            enemy.GetComponent<Enemy>().Init(spawnData[Random.Range(GameManager.Instance.CurStage-3, GameManager.Instance.CurStage)]);

        }
        else {
            enemy.GetComponent<Enemy>().Init(spawnData[GameManager.Instance.CurStage]);
        }

    }
    void SpawnElite() {
        GameObject enemy = GameManager.Instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 1 ˻ϴ  : GetComponentsInChildern 0°ε ڱڽ̴.
        if (GameManager.Instance.CurStage % 4 == 3) {
            enemy.GetComponent<Enemy>().InitElite(spawnData[Random.Range(GameManager.Instance.CurStage-3, GameManager.Instance.CurStage)]);

        }
        else {
            enemy.GetComponent<Enemy>().InitElite(spawnData[GameManager.Instance.CurStage]);
        }
    }

    public void SpawnMiddleBoss(int index)
    {
        GameObject enemy = Instantiate(middleBossList[index]);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 1 ˻ϴ  : GetComponentsInChildern 0°ε ڱڽ̴.
    }
}


[System.Serializable]
public class SpawnData {

    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}
