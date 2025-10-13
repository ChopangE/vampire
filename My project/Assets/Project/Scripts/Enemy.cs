using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using InGame;
using InGame.Data;
using Manager;
using UnityEngine;

public class Enemy : DamageObject
{
    enum EnemyType
    {
        Elite, Normal,
    }

    public float speed;
    public Transform[] dropItemSpawnPoints;
    public RuntimeAnimatorController[] animCon;
    public RuntimeAnimatorController[] eliteanimCon;
    public Rigidbody2D target;
    public OutlineSprite outlineSprite;

    private EnemyType _enemyType = EnemyType.Normal;
    protected bool isLive;
    int level;
    protected float timer;

    protected Rigidbody2D rigid;
    protected Collider2D coll;
    protected Animator anim;
    protected SpriteRenderer spriter;
    protected WaitForFixedUpdate wait;
    protected Vector2 targetVec;

    public bool canStun = true;
    private bool _isStunned;
    public bool isStunned
    {
        get => _isStunned;
        set
        {
            if (canStun)
            {
                _isStunned = value;
                outlineSprite.SetStunned(value);

                // 스턴 상태일 때는 이동 불가
                if (value)
                {
                    rigid.velocity = Vector2.zero;
                }
            }
        }
    }

    public WeaponController weaponController;


    private float lastDamageTime = 0f; // 마지막으로 데미지를 받은 시간
    private const float DAMAGE_INTERVAL = 1f; // 데미지 간격

    private const string Floor = "Floor"; // "Floor" 태그를 상수로 정의 
    private const string Hit = "Hit"; // "Hit" 애니메이션 상태 이름을 상수로 정의
    private const string NormalEnemyTag = "NormalEnemy"; // 일반 몹 처치 카운트용 태그
    // Cached/shared resources to avoid repeated expensive calls when many enemies die
    private static AchievementsManager cachedAchievementsManager;
    private static DropItem cachedGoldDropItem;
    private static DropItem cachedLevelUpScroll;

    // Batched save for kill counts to avoid disk/serialization thrashing
    private static int pendingNormalKillIncrements = 0;
    private static float lastNormalKillSaveTime = 0f;
    private const int NormalKillSaveThreshold = 20; // save after this many kills
    private const float NormalKillSaveInterval = 30f; // or this many seconds
    protected override void Awake()
    {
        base.Awake();
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        weaponController = GameManager.Instance.weaponController;
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isLive) 
        {
            rigid.velocity = Vector2.zero;
            return;
        }
        if (!isLive) return;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(Hit)) return;
        if (_isStunned) return; // 스턴 상태일 때는 이동하지 않음

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextVec + rigid.position);
        //transform.Translate(nextVec);
        //rigid.velocity = Vector2.zero;


    }

    void LateUpdate()
    {
        if (!GameManager.Instance.isLive) 
        {
            rigid.velocity = Vector2.zero;
            return;
        }

        if (isLive)
        {
            spriter.flipX = target.position.x < rigid.position.x;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
        isStunned = false; // 활성화될 때 스턴 해제
    }

    public void Init(SpawnData data)
    {
        base.OnEnable();
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = 10 * Mathf.Pow(1.2f, GameManager.Instance.CurStage);
        health = maxHealth;
        level = data.spriteType;
        _enemyType = EnemyType.Normal;
    }
    public void InitElite(SpawnData data)
    {
        anim.runtimeAnimatorController = eliteanimCon[data.spriteType];
        speed = data.speed;
        maxHealth = 10 * Mathf.Pow(1.2f, GameManager.Instance.CurStage) * 20;
        health = maxHealth;
        level = data.spriteType;
        _enemyType = EnemyType.Elite;

    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        // TryGetComponent once and use the result; avoid extra GetComponent calls and tag checks
        if (!collision.TryGetComponent(out Bullet bullet)) return;

        Global.SoundManager.PlayHitSFX(Data.SFXEnum.Monster_Hit_1, isShootCooldown: false);

        if (bullet is SecretSmash)
        {
            // 일반 몹과 엘리트 몹만 즉사 처리
            if (_enemyType == EnemyType.Elite || _enemyType == EnemyType.Normal)
            {
                isLive = false;
                coll.enabled = false;
                rigid.simulated = false;
                spriter.sortingOrder = 1;
                Dead();
                return;
            }
        }
        else
        {
            // use the already obtained bullet variable
            CalculateDamage(bullet.CalculateDamage());
        }

        if (health > 0)
        {
            anim.SetTrigger(Hit);
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            Dead();
        }
    }

    private void CalculateDamage(DamageData damageData)
    {
        float finalDamage = damageData.damage;
        bool isCritical = damageData.isCritical;

        finalDamage = finalDamage * (1 + GameManager.Instance.player.damageBonus);

        // Bullet에서 이미 크리티컬 계산이 되어있으므로, 여기서는 그 결과만 사용합니다
        health -= finalDamage;
        GameManager.DamageTextPoolManager.SpawnDamageText(transform.position, finalDamage, isCritical, false);

        if (weaponController.isBonusDamage && damageData.isBonus)
        {
            float bonusDamage = finalDamage * weaponController.bonusDamage;
            health -= bonusDamage;
            DelayedSpawnDamageText(transform.position, bonusDamage, 0.1f, true).Forget();
        }
    }

    private async UniTaskVoid DelayedSpawnDamageText(Vector3 position, float damage, float delay, bool isBonus = false)
    {
        await UniTask.Delay((int)(delay * 1000));
        GameManager.DamageTextPoolManager.SpawnDamageText(transform.position, damage, false, isBonus);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Circle"))
        {
            Vector2 dir = targetVec - rigid.position;
            anim.SetTrigger("Hit");
            rigid.AddForce(dir.normalized * 7, ForceMode2D.Impulse);

        }
    }
    public void GetDamage(float damage)
    {
        health -= damage;
        GameManager.DamageTextPoolManager.SpawnDamageText(transform.position, damage);

        if (health > 0)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            //anim.SetBool("Dead", true);
            Dead();
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag(Floor)) return;

        if (!collision.TryGetComponent(out Bullet bullet)) return;  // Bullet 컴포넌트 체크 추가

        // 현재 시간이 마지막 데미지 시간 + 간격보다 큰 경우에만 데미지 적용
        if (Time.time >= lastDamageTime + DAMAGE_INTERVAL)
        {
            CalculateDamage(bullet.CalculateDamage());
            lastDamageTime = Time.time;
            if (health > 0)
            {
                anim.SetTrigger(Hit);
            }
            else
            {
                isLive = false;
                coll.enabled = false;
                rigid.simulated = false;
                spriter.sortingOrder = 1;
                Dead();
            }
        }

        // if (collision.TryGetComponent(out WhirlBullet whirlBullet))
        // {
        //     targetVec = collision.GetComponent<Rigidbody2D>().position;
        //     if (gameObject.activeSelf) StartCoroutine(KnockBack());
        // }
        

    }
    IEnumerator KnockBack()
    {
        Vector2 dir = targetVec - rigid.position;
        yield return wait;          // 1물리 프레임 wait
        rigid.AddForce(dir.normalized * 5, ForceMode2D.Impulse);
        yield return wait;          // 1물리 프레임 wait
    }

    public override void Dead()
    {
        // GameObject coin = GameManager.Instance.pool.Get(coinNum);
        // coin.transform.position = transform.position;
        // coin.transform.rotation = Quaternion.identity;
        // Coin cc = coin.GetComponent<Coin>();
        // cc.sprite.sprite = cc.sprites[Mathf.Min((level / 4),cc.sprites.Length-1)];
        // cc.exp = level + 1;                               //경험치 조절 여기서 가능
        if (_enemyType == EnemyType.Normal)
        {
            // Spawn exp (cheap)
            Global.ExpManager.SpawnExpItem(level / 4 + 1, transform.position);

            // Batch increment normal enemy kill counter to avoid heavy per-death saves
            var userDataManager = Manager.Global.UserDataManager;
            if (userDataManager != null)
            {
                pendingNormalKillIncrements++;
                // apply to storage immediately in memory
                var killSaveData = userDataManager.storage.killSaveData;
                if (killSaveData.ContainsKey(NormalEnemyTag))
                    killSaveData[NormalEnemyTag] += 1;
                else
                    killSaveData[NormalEnemyTag] = 1;

                // Save only when threshold reached or interval elapsed
                if (pendingNormalKillIncrements >= NormalKillSaveThreshold || Time.time - lastNormalKillSaveTime >= NormalKillSaveInterval)
                {
                    pendingNormalKillIncrements = 0;
                    lastNormalKillSaveTime = Time.time;
                    userDataManager.Save();
                }

                // Cache AchievementsManager lookup (expensive) and run checks without further allocations
                if (cachedAchievementsManager == null)
                    cachedAchievementsManager = GameObject.FindObjectOfType<AchievementsManager>();

                if (cachedAchievementsManager != null)
                {
                    int curKills = killSaveData.ContainsKey(NormalEnemyTag) ? killSaveData[NormalEnemyTag] : 0;
                    if (curKills >= 1000) cachedAchievementsManager.AchivementTrueByID("1,000_Enemy_Kills");
                    if (curKills >= 2000) cachedAchievementsManager.AchivementTrueByID("2,000_Enemy_Kills");
                    if (curKills >= 5000) cachedAchievementsManager.AchivementTrueByID("5,000_Enemy_Kills");
                }
            }
        }
        else
        {
            Global.ExpManager.SpawnExpItem(0, transform.position);
            // Cache DropItem prefabs to avoid repeated Resources.Load during mass deaths
            if (cachedGoldDropItem == null)
                cachedGoldDropItem = Resources.Load<DropItem>("Prefabs/DropItem/GoldBarCoinGold");
            if (cachedLevelUpScroll == null)
                cachedLevelUpScroll = Resources.Load<DropItem>("Prefabs/DropItem/SpecialItem/LevelUpScroll");

            Vector3 dropPosition = dropItemSpawnPoints != null && dropItemSpawnPoints.Length > 0
                ? dropItemSpawnPoints[0].position
                : transform.position;
            GameManager.DropItemPoolManager.SpawnDropItem(cachedGoldDropItem, dropPosition);

            Vector3 dropPosition2 = dropItemSpawnPoints != null && dropItemSpawnPoints.Length > 1
                ? dropItemSpawnPoints[1].position
                : transform.position;
            GameManager.DropItemPoolManager.SpawnDropItem(cachedLevelUpScroll, dropPosition2);
        }
        GameManager.Instance.kill++;
        gameObject.SetActive(false);
    }
    public void GetAddForce(Vector3 pos)
    {
        Vector2 enemyToCircle = (pos - transform.position).normalized;
        rigid.AddForce(enemyToCircle * 10, ForceMode2D.Impulse);
    }

}
