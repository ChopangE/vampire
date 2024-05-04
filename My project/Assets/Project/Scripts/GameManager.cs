using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
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
    void Awake() {
        var objs = FindObjectsOfType<GameManager>();
        if(objs.Length != 1) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
       
    }
    void Start() {
        health = maxHealth;
        AudioManager.instance.PlayBgm(true);
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
