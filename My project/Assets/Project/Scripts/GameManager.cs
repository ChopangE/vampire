using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    

    [Header("# Datas")]
    public playerData[] datas;
    public Weapon[] weapons;
    public FloorWeapon floorWeapon;
    public Item[] items;
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
        if(StageManager.Instance.curPoint == 1) {           //On stage �ӽ��ڵ�
            player.transform.position = Vector3.zero;
            bossLevel.SetActive(false);
            spawner.SetActive(true);
            timer.SetActive(true);
        }
        else {
            player.transform.position = new Vector3(100,98,0);
            bossLevel.SetActive(true);
            spawner.SetActive(false);
            timer.SetActive(false);

        }
    }
    void GetData() {
        for(int i= 0; i < 5; i++) {
            if (datas[i].isHave) {
                weapons[i].gameObject.SetActive(true);
                weapons[i].damage = datas[i].damage;
                weapons[i].count = datas[i].count;
                items[i].level = datas[i].level;
                weapons[i].InitSetting();
            }
        }
        if (datas[5].isHave) {
            floorWeapon.gameObject.SetActive(true);
            floorWeapon.damage = datas[5].damage;
            items[5].level = datas[5].level;
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
            datas[5].isHave = floorWeapon.gameObject.activeSelf;
            datas[5].damage = floorWeapon.damage;
            datas[5].count = 0;
            datas[5].level = items[5].level;
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
