using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance {
        get {
            if(instance == null) {
                var obj = FindObjectOfType<GameManager>();
                if(obj != null) {
                    instance = obj;
                }
                else {
                    var newObj = new GameObject().AddComponent<GameManager>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }
    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    [Header("# Player Info")]
    public float health;
    public float maxHealth = 100;
    public int[] nextExp = { 5, 10, 20, 30 };
    public int level = 0;
    public int kill = 0;
    public int coin = 0;
    [Header("# Game Object")]
    public Player player;
    public PoolManager pool;
    public LevelUp uiLevelUp;
    public Pause uiPause;
    public GameObject spawner;
    public GameObject timer;
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
    public Transform[] stages = new Transform[StageManager.Instance.maxStageNum * StageManager.Instance.maxStageCountNum];
    public int curStage;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        var objs = FindObjectsOfType<GameManager>();
        if(objs.Length != 1) {
            Destroy(gameObject);
            return;
        }
       
    }
    void Start() {
        health = maxHealth;
        AudioManager.instance.PlayBgm(true);
        datas = DataManager.Instance.GetData();
        weapons = player.GetComponentsInChildren<Weapon>(true);
        floorWeapon = player.GetComponentInChildren<FloorWeapon>(true);
        //item은 우선 inspector에서 집어넣음.
        GetData();
        //여기에서 설정해주기
        if (StageManager.Instance.stageCount == StageManager.Instance.maxStageCountNum) {
            StageManager.Instance.stageLevel++;
        }
        curStage = StageManager.Instance.stageCount++;
        player.transform.position = stages[curStage].position;
        if (curStage == StageManager.Instance.maxStageCountNum * StageManager.Instance.maxStageNum) {
            bossLevel.SetActive(true);
            spawner.SetActive(false);
            timer.SetActive(false);
        }
        else {
            bossLevel.SetActive(false);
            spawner.SetActive(true);
            timer.SetActive(true);
        }

        //if(StageManager.Instance.curPoint == 1) {           //On stage �ӽ��ڵ�
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
            uiPause.Show();
        }

    }
    public void GetExp(int exp)
    {
        coin += exp;
        if (coin >= nextExp[level])
        {
            level = Mathf.Min(level + 1, nextExp.Length-1);
            coin = 0;
            uiLevelUp.Show();
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
