using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Manager;
using UI.Page;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MMSingleton<GameManager>
{
    #region Static Properties
    public static DropItemPoolManager DropItemPoolManager { get; set; }
    public static DamageTextPoolManager DamageTextPoolManager { get; set; }
    public static PassiveManager PassiveManager { get; set; }
    #endregion

    #region Serialized Fields
    [Header("Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;

    [Header("Player Stats")]
    public float maxHealth = 100;
    public float baseMaxHealth = 100;
    public float baseDefense = 0;
    public float criticalDamage = 1.5f;
    public float baseCriticalDamage = 1.5f;
    public float criticalChance = 0f;
    public float baseCriticalChance = 0f;
    public float expBonus = 1.0f;
    public float expRangeBonus = 1.0f;

    [Header("Level System")]
    public int[] nextExp = { 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
    public int level;
    public int kill;
    public int curExp;

    [Header("References")]
    public WeaponController weaponController;
    public Player player;
    public PoolManager pool;
    public GameObject spawner;
    public GameObject shieldObject;
    public GameObject bossLevel;
    public Transform[] stages;
    #endregion

    #region Private Fields
    private float _health;
    private float _shield;
    private float _defense;
    private int _curStage;
    private InGameMainPage _inGameMainPage;
    #endregion

    #region Properties
    public float Defense
    {
        get => _defense;
        set => _defense = Mathf.Max(0, value);
    }
    public float Health
    {
        get => _health;
        set
        {
            float damage = _health - value;
            if (damage <= 0)
            {
                _health = Mathf.Clamp(value, 0, maxHealth);
                return;
            }

            float reducedDamage = damage * (100f - _defense) / 100f;
            reducedDamage = Mathf.Max(0, reducedDamage);

            if (_shield > 0)
            {
                if (_shield >= reducedDamage)
                {
                    Shield -= reducedDamage;
                    return;
                }
                else
                {
                    float remainingDamage = reducedDamage - _shield;
                    Shield = 0;
                    _health = Mathf.Max(0, _health - remainingDamage);
                }
            }
            else
            {
                _health = Mathf.Max(0, _health - reducedDamage);
            }

            if (_health <= 0)
            {
                GameOver();
            }
        }
    }


    public float Shield
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

    public int CurStage
    {
        get => _curStage;
        set => _curStage = value;
    }
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        InitializeGame();
        SetupStage();
    }

    private void Update()
    {
        if (!isLive) return;

        gameTime = Mathf.Min(gameTime + Time.deltaTime, maxGameTime);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _inGameMainPage.Pause();
        }
    }
    #endregion

    #region Game State Methods
    private void InitializeGame()
    {
        DropItemPoolManager = FindObjectOfType<DropItemPoolManager>();
        DamageTextPoolManager = FindObjectOfType<DamageTextPoolManager>();
        PassiveManager = FindObjectOfType<PassiveManager>();
        _inGameMainPage = Global.UIManager.OpenPage<InGameMainPage>();


        _health = maxHealth;
        _shield = 0;
        _defense = baseDefense;
        PassiveManager.Init();
    }

    private void SetupStage()
    {
        _curStage = Global.UserDataManager.curStage++;
        player.transform.position = stages[_curStage].position;

        if (_curStage == Global.StageManager.MAX_STAGE_COUNT * Global.StageManager.MAX_STAGE_LEVEL)
        {
            SetupFinalBossStage();
        }
        else
        {
            SetupNormalStage();
        }
    }

    private void SetupFinalBossStage()
    {
        bossLevel.SetActive(true);
        spawner.SetActive(false);
        _inGameMainPage.ActiveTimer = false;
    }

    private void SetupNormalStage()
    {
        if (_curStage % Global.StageManager.MAX_STAGE_COUNT == Global.StageManager.MAX_STAGE_COUNT - 1)
        {
            SpawnStageBoss().Forget();
        }
        bossLevel.SetActive(false);
        spawner.SetActive(true);
        _inGameMainPage.ActiveTimer = true;

    }

    private async UniTaskVoid SpawnStageBoss()
    {
        GameObject bossTran = await pool.GetAsync(11 + Global.StageManager.stageCount);
        Global.StageManager.ChangeStage(Global.UserDataManager.curStage + 1);
        bossTran.transform.position = stages[_curStage].position + new Vector3(0, 10f, 0);
    }




    #endregion

    #region Game Progress Methods
    public void GetExp(int exp)
    {
        curExp += (int)(exp * expBonus);
        if (curExp >= nextExp[level])
        {
            curExp = 0;
            LevelUp();
        }
    }

    public void LevelUp(bool isEvaluation = false)
    {
        level = Mathf.Min(level + 1, nextExp.Length - 1);
        ShowLevelUp(isEvaluation);
    }

    public void ShowLevelUp(bool isEvaluation = false)
    {
        _inGameMainPage.ShowLevelUP(isEvaluation);
    }
    #endregion

    #region Game Control Methods
    public void StageClear()
    {
        Global.UserDataManager.curStage++;
        Global.UserDataManager.Save();
        Global.DataManager.SaveData();
        Global.UIManager.CloseAllPages();
        SceneManager.LoadScene(3);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);
        Stop();
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
    #endregion

    #region Stage Utility Methods
    public Transform CurStagePos()
    {
        if (stages == null || _curStage < 0 || _curStage >= stages.Length)
        {
            Debug.LogError($"유효하지 않은 스테이지 인덱스: {_curStage}");
            return null;
        }
        return stages[_curStage];
    }

    public Bounds CurStageBounds()
    {
        Transform currentStage = CurStagePos();
        if (currentStage == null) return new Bounds();

        CompositeCollider2D collider = currentStage.GetComponentInChildren<CompositeCollider2D>();
        if (collider == null)
        {
            Debug.LogError("CompositeCollider2D를 찾을 수 없습니다.");
            return new Bounds();
        }
        return collider.bounds;
    }
    #endregion
}
