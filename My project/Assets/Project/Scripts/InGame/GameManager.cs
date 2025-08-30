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
    public float gameTime = 0;
    public float maxGameTime = 2 * 10f;

    [Header("Player Stats")]
    public float maxHealth = 100;
    public float baseMaxHealth = 100;
    public float baseDefense = 0;
    public float criticalDamage = 1.5f;
    public float baseCriticalDamage = 1.5f;
    public float criticalChance = 0f;
    public float baseCriticalChance = 0f;
    public float revivePossibility = 0f;
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
    public float BossHealth { get; set; }
    public float maxBossHealth { get; set; }
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
            if (!isLive) return;
            float damage = _health - value;
            if (damage <= 0)
            {
                _health = value;
                return;
            }

            // 방어력을 직접 퍼센트로 적용 (방어력 10 = 10% 데미지 감소)
            float damageReductionPercent = Mathf.Clamp(_defense, 0f, 100f) / 100f; // 최대 100% 방어
            float reducedDamage = damage * (1f - damageReductionPercent);
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
                    _health = _health - remainingDamage;
                }
            }
            else
            {
                Global.SoundManager.PlayHitSFX(Data.SFXEnum.HGD_Hit);
                _health = _health - reducedDamage;
            }

            if (_health <= 0)
            {
                _health = 0; // 체력을 0으로 고정
                GameOver();
            }
        }
    }
    public bool isInvincible { get; set; }

    public float goldBonus = 1.0f;
    public float projectileSpeedBonus = 1.0f;
    public float healthRegeneration = 0.0f;


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
        
        // 필수 컴포넌트들이 할당되어 있는지 확인
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        if (spawner == null)
        {
            spawner = FindObjectOfType<Spawner>();
        }
        if (weaponController == null)
        {
            weaponController = FindObjectOfType<WeaponController>();
        }
        if (pool == null)
        {
            pool = FindObjectOfType<PoolManager>();
        }
        
        InitializeGame();
        SetupStage();
    }

    private void Update()
    {
        if (!isLive) return;

        gameTime = Mathf.Min(gameTime + Time.deltaTime, maxGameTime);
        if (Input.GetKeyDown(KeyCode.Escape) && gameTime > 1.5f && _inGameMainPage != null)
        {
            _inGameMainPage.Pause();
        }
        Health += healthRegeneration * Time.deltaTime;
    }
    #endregion

    #region Game State Methods
    private void InitializeGame()
    {
        DropItemPoolManager = FindObjectOfType<DropItemPoolManager>();
        DamageTextPoolManager = FindObjectOfType<DamageTextPoolManager>();
        PassiveManager = FindObjectOfType<PassiveManager>();
        
        // UIManager가 초기화되어 있는지 확인
        if (Global.UIManager != null)
        {
            _inGameMainPage = Global.UIManager.OpenPage<InGameMainPage>();
        }
        else
        {
            Debug.LogError("Global.UIManager가 초기화되지 않았습니다.");
        }

        maxHealth = baseMaxHealth;
        Health = maxHealth;
        _shield = 0;
        _defense = baseDefense;
        level = Global.UserDataManager.level; // 저장된 레벨 불러오기
        
        if (PassiveManager != null)
        {
            PassiveManager.Init();
        }
        else
        {
            Debug.LogWarning("PassiveManager를 찾을 수 없습니다.");
        }
    }

    private void SetupStage()
    {
        _curStage = Global.UserDataManager.curStage;
        
        if (player == null || stages == null || _curStage < 0 || _curStage >= stages.Length)
        {
            Debug.LogError($"SetupStage 실패: player={player}, stages={stages}, stages.Length={stages?.Length}, _curStage={_curStage}");
            // 필수 컴포넌트들을 다시 찾아보기
            StartCoroutine(RetrySetupStage());
            return;
        }
        
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
    
    private IEnumerator RetrySetupStage()
    {
        int retryCount = 0;
        const int maxRetries = 10;
        
        while (retryCount < maxRetries)
        {
            yield return new WaitForSeconds(0.1f); // 0.1초 대기
            
            // 필수 컴포넌트들을 다시 찾기
            if (player == null)
                player = FindObjectOfType<Player>();
            if (stages == null || stages.Length == 0)
            {
                // stages 배열을 다시 찾는 로직이 필요하면 여기에 추가
                // 현재는 Inspector에서 할당되는 것으로 보임
            }
            
            // 모든 필수 컴포넌트가 준비되었으면 다시 시도
            if (player != null && stages != null && _curStage >= 0 && _curStage < stages.Length)
            {
                SetupStage();
                yield break;
            }
            
            retryCount++;
        }
        
        Debug.LogError($"SetupStage 재시도 실패: {maxRetries}번 시도 후에도 필수 컴포넌트를 찾을 수 없습니다.");
    }

    private void SetupFinalBossStage()
    {
        if (bossLevel != null)
            bossLevel.SetActive(true);
        else
            Debug.LogWarning("bossLevel이 할당되지 않았습니다.");
            
        if (spawner != null)
            spawner.gameObject.SetActive(false);
        else
            Debug.LogWarning("spawner가 할당되지 않았습니다.");
            
        if (_inGameMainPage != null)
            _inGameMainPage.ActiveTimer = false;
    }

    private void SetupNormalStage()
    {
        if (bossLevel != null)
            bossLevel.SetActive(false);
        else
            Debug.LogWarning("bossLevel이 할당되지 않았습니다.");
            
        if (spawner != null)
            spawner.gameObject.SetActive(true);
        else
            Debug.LogWarning("spawner가 할당되지 않았습니다.");
            
        if (_curStage % Global.StageManager.MAX_STAGE_COUNT == Global.StageManager.MAX_STAGE_COUNT - 1)
        {
            int bossIndex = 0;
            if (_curStage == 3) bossIndex = 0;
            else if (_curStage == 7) bossIndex = 1;
            else if (_curStage == 11) bossIndex = 2;
            
            if (spawner != null)
                spawner.SpawnMiddleBoss(bossIndex);
            else
                Debug.LogError("spawner가 null이어서 보스를 소환할 수 없습니다.");
                
            if (_inGameMainPage != null)
                _inGameMainPage.ActiveTimer = false;
        }
        else
        {
            if (_inGameMainPage != null)
                _inGameMainPage.ActiveTimer = true;
        }
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
        Global.UserDataManager.level = level; // 레벨 저장 (프로퍼티가 자동으로 Save 호출)
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
        if (_inGameMainPage != null)
        {
            _inGameMainPage.ShowLevelUP(isEvaluation);
        }
        else
        {
            Debug.LogWarning("InGameMainPage가 null입니다. ShowLevelUp을 호출할 수 없습니다.");
        }
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
        if (isStageClear) return;
        isStageClear = true;

        // 최종 스테이지 클리어 여부 확인 (데이터 초기화 전에)
        bool isFinalStageClear = _curStage == 12;


        var achievementsManager = GameObject.FindObjectOfType<AchievementsManager>();
        // 마지막 스테이지 클리어 시 First_Game_Clear 업적 클리어
        if (isFinalStageClear && achievementsManager != null)
        {
            achievementsManager.AchivementTrueByID("First_Game_Clear");
        }

        // 클리어 시 체력이 만피면 Perfect_Stage_Clear 업적 클리어
        if (_health == maxHealth && achievementsManager != null)
        {
            achievementsManager.AchivementTrueByID("Perfect_Stage_Clear");
        }

        Global.UserDataManager.curStage++;
        Global.DataManager.SaveData();
        Global.SoundManager.PlaySFX(Data.SFXEnum.Shop_StageOpen);
        StartCoroutine(StageClearRoutine(isFinalStageClear));
    }

    public void GameOver()
    {
        Global.SoundManager.PlaySFX(Data.SFXEnum.HGD_Death);

        // 부활 확률 체크 (revivePossibility가 백분율로 되어있음, 예: 20은 20%를 의미)
        if (revivePossibility > 0 && UnityEngine.Random.Range(0f, 100f) < revivePossibility)
        {
            Global.SoundManager.PlaySFX(Data.SFXEnum.Revive);
            Health = maxHealth;
            Resume();
            return;
        }

        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);
        Stop();
        // 부활 실패 시 게임오버 처리
        Global.DataManager.ResetWeaponData();
        Global.UserDataManager.ResetPurchasedShopItems();
        Global.UserDataManager.ResetPassiveItemData();
        Global.UserDataManager.ResetStageData();
        
        var pages = Global.UIManager.GetPages<InGameMainPage>();
        if (pages != null && pages.Count > 0 && pages[0] != null)
        {
            FadeScript fade = pages[0].GetComponent<FadeScript>();
            if (fade != null)
            {
                fade.InGameFade(true);
            }
            else
            {
                Debug.LogError("FadeScript를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("InGameMainPage를 찾을 수 없습니다.");
        }
    }

    private IEnumerator StageClearRoutine(bool isFinalStageClear = false)
    {
        yield return new WaitForSeconds(0.5f);
        Stop();
        var pages = Global.UIManager.GetPages<InGameMainPage>();
        if (pages != null && pages.Count > 0 && pages[0] != null)
        {
            FadeScript fade = pages[0].GetComponent<FadeScript>();
            if (fade != null)
            {
                fade.InGameFade(isGameWin: true, isGameClear: isFinalStageClear);
            }
            else
            {
                Debug.LogError("FadeScript를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("InGameMainPage를 찾을 수 없습니다.");
        }
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
