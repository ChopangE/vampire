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
    public Spawner spawner;
    public GameObject shieldObject;
    public GameObject bossLevel;
    public Transform[] stages;
    public bool isStageClear = false;
    #endregion

    #region Private Fields
    private float _health;
    private float _shield;
    private float _defense;
    private int _curStage;
    private InGameMainPage _inGameMainPage;
    private Queue<bool> _levelUpQueue = new Queue<bool>();  // 레벨업 대기열
    private bool _isProcessingLevelUp;  // 레벨업 UI 처리 중인지 여부
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
            if(!isLive) return;
            float damage = _health - value;
            if (damage <= 0)
            {
                _health = Mathf.Clamp(value, 0, maxHealth);
                return;
            }

            float reducedDamage = damage * (100f - _defense) / 100f;
            reducedDamage = Mathf.Max(0, reducedDamage);
            if (isInvincible)
            {
                reducedDamage = 0;
            }
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
                Global.SoundManager.PlayHitSFX(Data.SFXEnum.HGD_Hit);
                _health = Mathf.Max(0, _health - reducedDamage);
            }

            if (_health <= 0)
            {
                GameOver();
            }
        }
    }
    public bool isInvincible { get; set; }
    


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
        
        maxHealth = baseMaxHealth;
        Health = maxHealth;
        _shield = 0;
        _defense = baseDefense;
        PassiveManager.Init();
    }

    private void SetupStage()
    {
        _curStage = Global.UserDataManager.curStage;
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
        spawner.gameObject.SetActive(false);
        _inGameMainPage.ActiveTimer = false;
    }

    private void SetupNormalStage()
    {
        bossLevel.SetActive(false);
        spawner.gameObject.SetActive(true);
        if (_curStage % Global.StageManager.MAX_STAGE_COUNT == Global.StageManager.MAX_STAGE_COUNT - 1)
        {
            int bossIndex = 0;
            if(_curStage == 3) bossIndex = 0;
            else if(_curStage == 7) bossIndex = 1;
            else if(_curStage == 11) bossIndex = 2;
            spawner.SpawnMiddleBoss(bossIndex);
            _inGameMainPage.ActiveTimer = false;
        }
        _inGameMainPage.ActiveTimer = true;

    }



    #endregion

    #region Game Progress Methods
    public void GetExp(int exp)
    {
        if (!isLive) return;
        
        curExp += (int)(exp * expBonus);
        Global.SoundManager.PlaySFX(Data.SFXEnum.GetExpStone);
        
        // 경험치가 충분하면 레벨업 처리
        while (curExp >= GetNextExpRequired())
        {
            curExp -= GetNextExpRequired();
            QueueLevelUp();
        }
        
        if (!_isProcessingLevelUp)
        {
            ProcessNextLevelUp();
        }
    }

    public int GetNextExpRequired()
    {
        // 최대 레벨에 도달했을 경우 마지막 경험치 요구량 반환
        return level >= nextExp.Length ? nextExp[nextExp.Length - 1] : nextExp[level];
    }
    public void QueueEvaluateLevelUp()
    {
        _levelUpQueue.Enqueue(true);  // 평가 레벨업
        ProcessNextLevelUp();
    }

    private void QueueLevelUp()
    {
        level += 1;  // 레벨은 계속 증가 (표시용)
        _levelUpQueue.Enqueue(false);  // 일반 레벨업
    }

    private void ProcessNextLevelUp()
    {
        if (_levelUpQueue.Count == 0) return;
        
        _isProcessingLevelUp = true;
        isLive = false;  // 레벨업 처리 중 게임 일시정지
        Global.SoundManager.PlaySFX(Data.SFXEnum.LevelUp);
        ShowLevelUp(_levelUpQueue.Dequeue());
    }

    public void ShowLevelUp(bool isEvaluation = false)
    {
        _inGameMainPage.ShowLevelUP(isEvaluation);
    }

    // InGameMainPage에서 레벨업 UI가 닫힐 때 호출할 메서드
    public void OnLevelUpComplete()
    {
        _isProcessingLevelUp = false;
        isLive = true;  // 게임 재개
        
        if (_levelUpQueue.Count > 0)
        {
            ProcessNextLevelUp();
        }
    }
    #endregion

    #region Game Control Methods
    public void StageClear()
    {
        if(isStageClear) return;
        isStageClear = true;
        Global.UserDataManager.curStage++;
        Global.DataManager.SaveData();
        Global.SoundManager.PlaySFX(Data.SFXEnum.Shop_StageOpen);
        StartCoroutine(StageClearRoutine());
    }

    public void GameOver()
    {
        Global.SoundManager.PlaySFX(Data.SFXEnum.HGD_Death);
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);
        Stop();
        var pages = Global.UIManager.GetPages<InGameMainPage>();
        FadeScript fade = pages[0].GetComponent<FadeScript>();
        fade.FadeOut(true);
    }

    private IEnumerator StageClearRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Stop();
        var pages = Global.UIManager.GetPages<InGameMainPage>();
        FadeScript fade = pages[0].GetComponent<FadeScript>();
        fade.FadeOut(isGameWin: true);
    }
    public void Stop()
    {
        isLive = false;
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
