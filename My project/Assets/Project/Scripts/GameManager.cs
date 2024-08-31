using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Manager;
using UI.Page;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MMSingleton<GameManager>
{

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    [Header("# Player Info")]
    public float health;
    public float maxHealth = 100;
    public int[] nextExp = { 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
    public int level = 0;
    public int kill = 0;
    public int coin = 0;
    [Header("# Game Object")]
    public Player player;
    public PoolManager pool;
    public GameObject spawner;
    [Header("# Boss Object")]
    public GameObject bossLevel;
    

    [Header("# Data")]
    public playerData[] datas;
    public Weapon[] weapons;
    public FloorWeapon floorWeapon;
    public Item[] items;
    
    [Header("# Stage Data")]
    //private static int maxStageNum = 3;
    //private static int maxStageCountNum = 4;
    //List<GameObject>[] stages = new List<GameObject>[maxStageNum];
    public Transform[] stages = new Transform[Global.StageManager.maxStageNum * Global.StageManager.maxStageCountNum];
    public int curStage;

    private InGameMainPage inGameMainPage;
    void Start() {
        //* 인게임 UI 호출
        Global.Create(true);
        inGameMainPage = Global.UIManager.OpenPage<InGameMainPage>();

        health = maxHealth;
        AudioManager.instance.PlayBgm(true);
        datas = DataManager.Instance.GetData();
        weapons = player.GetComponentsInChildren<Weapon>(true);
        floorWeapon = player.GetComponentInChildren<FloorWeapon>(true);
        //item은 우선 inspector에서 집어넣음.
        GetData();
        //여기에서 설정해주기
        //if (StageManager.Instance.stageCount % StageManager.Instance.maxStageCountNum == StageManager.Instance.maxStageCountNum - 1) {
        //    StageManager.Instance.stageLevel++;
        //}
        curStage = Global.StageManager.stageCount++;
        player.transform.position = stages[curStage].position;
        if (curStage == Global.StageManager.maxStageCountNum * Global.StageManager.maxStageNum) {
            bossLevel.SetActive(true);
            spawner.SetActive(false);
            inGameMainPage.ActiveTimer = false;
        }
        else {
            if(curStage % Global.StageManager.maxStageCountNum == Global.StageManager.maxStageCountNum - 1) {
                Debug.Log("STargt");
                Transform bossTran = pool.Get(11 + Global.StageManager.stageLevel++).transform;
                bossTran.position = stages[curStage].position + new Vector3(0,10f,0);
                //보스 소환
            }
            bossLevel.SetActive(false);
            spawner.SetActive(true);
            inGameMainPage.ActiveTimer = true;
        }

        //if(Global.StageManager.curPoint == 1) {           //On stage �ӽ��ڵ�
        //    player.transform.position = Vector3.zero;
        //    bossLevel.SetActive(false);
        //    spawner.SetActive(true);
        //    timer.SetActive(true);
        //}
        //else {
        //    player.transform.position = new Vector3(100,98,0);
        //    bossLevel.SetActive(true);
        //    spawner.SetActive(false);
        //    timer.SetActive(false);
        //}
    }
    void GetData() {
        for(int i= 0; i < 6; i++) {
            if (datas[i].isHave) {
                weapons[i].gameObject.SetActive(true);
                weapons[i].damage = datas[i].damage;
                weapons[i].count = datas[i].count;
                items[i].level = datas[i].level;
                weapons[i].InitSetting();
            }
        }
        if (datas[6].isHave) {
            floorWeapon.gameObject.SetActive(true);
            floorWeapon.damage = datas[6].damage;
            items[6].level = datas[6].level;
        }
    }

    void SetData() {
        for(int i = 0; i < weapons.Length; i++) {
            if (weapons[i].gameObject.activeSelf) {
                datas[i].isHave = weapons[i].gameObject.activeSelf;
                datas[i].damage = weapons[i].damage;
                datas[i].count = weapons[i].count;
                datas[i].level = items[i].level;
            }
        }
        if (floorWeapon.gameObject.activeSelf) {
            datas[6].isHave = floorWeapon.gameObject.activeSelf;
            datas[6].damage = floorWeapon.damage;
            datas[6].count = 0;
            datas[6].level = items[6].level;
        }
        DataManager.Instance.SetData(datas);
    }
    public void StageClear() {
        SetData();
        SceneManager.LoadScene(3);
    }
    public void GameOver() {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine() {

        isLive = false;
        yield return new WaitForSeconds(0.5f);
        Stop();

        AudioManager.instance.PlayBgm(false);
    }
    void Update() {
        if (!isLive) return;
        gameTime += Time.deltaTime;
        if(gameTime > maxGameTime) {
            gameTime = maxGameTime;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            inGameMainPage.Pause();
        }

    }
    public void GetExp(int exp)
    {
        coin += exp;
        if (coin >= nextExp[level])
        {
            level = Mathf.Min(level + 1, nextExp.Length-1);
            coin = 0;
            inGameMainPage.ShowLevelUP();
        }
        
    }
    
    public void Stop() {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume() {

        isLive = true;
        Time.timeScale = 1;

    }

}
