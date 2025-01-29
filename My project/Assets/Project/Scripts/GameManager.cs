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
    public static DropItemPoolManager DropItemPoolManager { get; set; }
    public static DamageTextPoolManager DamageTextPoolManager { get; set; }

    public WeaponController weaponController;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    [Header("# Player Info")]
    private float _health;
    private float _shield;
    public float maxHealth = 100;
    public int[] nextExp = { 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
    public int level = 0;
    public int kill = 0;
    public int curExp = 0;
    [Header("# Game Object")]
    public Player player;
    public PoolManager pool;
    public GameObject spawner;
    public GameObject shieldObject;
    [Header("# Boss Object")]
    public GameObject bossLevel;

    [Header("# Stage Data")]
    //private static int maxStageNum = 3;
    //private static int maxStageCountNum = 4;
    //List<GameObject>[] stages = new List<GameObject>[maxStageNum];
    public Transform[] stages;
    public int curStage;
    
    private InGameMainPage inGameMainPage;
    protected override void Awake() {
        base.Awake();
        //* 인게임 UI 호출
        DropItemPoolManager = FindObjectOfType<DropItemPoolManager>();
        DamageTextPoolManager = FindObjectOfType<DamageTextPoolManager>();
        inGameMainPage = Global.UIManager.OpenPage<InGameMainPage>();

        _health = maxHealth;
        _shield = 0;
        
        curStage = Global.UserDataManager.curStage++;
        player.transform.position = stages[curStage].position;
        if (curStage == Global.StageManager.MAX_STAGE_COUNT * Global.StageManager.MAX_STAGE_LEVEL) {
            bossLevel.SetActive(true);
            spawner.SetActive(false);
            inGameMainPage.ActiveTimer = false;
            //최종 보스 소환
        }
        else {
            if(curStage % Global.StageManager.MAX_STAGE_COUNT == Global.StageManager.MAX_STAGE_COUNT - 1) {
                Transform bossTran = pool.Get(11 + Global.StageManager.stageCount).transform;
                Global.UserDataManager.curStage++;
                bossTran.position = stages[curStage].position + new Vector3(0,10f,0);
                //보스 소환
            }
            bossLevel.SetActive(false);
            spawner.SetActive(true);
            inGameMainPage.ActiveTimer = true;
        }
    }


    public void StageClear() {
        DataManager.Instance.SaveData();
        Global.UIManager.CloseAllPages();
        SceneManager.LoadScene(3);
    }
    public void GameOver() {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine() {

        isLive = false;
        yield return new WaitForSeconds(0.5f);
        Stop();
        
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
        curExp += exp; 
        if (curExp >= nextExp[level])
        {
            curExp = 0;
            LevelUp();
        }
        
    }

    public void LevelUp(bool isEvaluation = false)
    {
        level = Mathf.Min(level + 1, nextExp.Length-1);
        ShowLevelUp(isEvaluation);
    }

    public void ShowLevelUp(bool isEvaluation = false)
    {
        inGameMainPage.ShowLevelUP(isEvaluation);

    }
    public void Stop() {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume() {

        isLive = true;
        Time.timeScale = 1;

    }

    public Transform CurStagePos() {
        if (stages == null || curStage < 0 || curStage >= stages.Length) {
            Debug.LogError($"유효하지 않은 스테이지 인덱스: {curStage}");
            return null;
        }
        return stages[curStage];
    }

    public Bounds CurStageBounds() {
        Transform currentStage = CurStagePos();
        if (currentStage == null) return new Bounds();
        
        CompositeCollider2D collider = currentStage.GetComponentInChildren<CompositeCollider2D>();
        if (collider == null) {
            Debug.LogError("CompositeCollider2D를 찾을 수 없습니다.");
            return new Bounds();
        }
        return collider.bounds;
    }

    public float health 
    {
        get => _health;
        set 
        {
            float damage = _health - value; // 받은 데미지 계산
            if (damage > 0 && shield > 0)   // 데미지를 받았고 쉴드가 있다면
            {
                if (shield >= damage)        // 쉴드가 데미지보다 크거나 같으면
                {
                    shield -= damage;        // 쉴드만 감소
                    return;                  // 체력은 감소하지 않음
                }
                else                        // 쉴드가 데미지보다 작으면
                {
                    float remainingDamage = damage - shield;
                    shield = 0;             // 쉴드를 모두 소진
                    _health -= remainingDamage; // 남은 데미지만큼 체력 감소
                }
            }
            else                           // 쉴드가 없거나 회복의 경우
            {
                _health = Mathf.Clamp(value, 0, maxHealth);
                // 체력이 0이 되면 게임오버
                if (_health <= 0)
                {
                    GameOver();
                }
            }
        }
    }

    public float shield
    {
        get => _shield;
        set
        {
            _shield = Mathf.Max(0, value);
            if (shieldObject != null)
            {
                shieldObject.SetActive(_shield > 0);
            }
        }
    }

}
