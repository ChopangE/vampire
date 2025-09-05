using Cinemachine;
using Cysharp.Threading.Tasks;
using Data;
using InGame.Data;
using Manager;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform damageTextSpawnPoint;
    private float _health;
    public float health
    {
        get => _health;
        set
        {
            _health = value;
            GameManager.Instance.BossHealth = _health;
        }
    }
    public float maxHealth;
    public Rigidbody2D target;
    bool isLive;
    public Animator anim;
    public Smash smash;
    public GameObject[] bossLevel;
    public float smashTime;
    CameraControl CC;
    Collider2D coll;
    SpriteRenderer sprite;
    BossWeapon[] weapons;
    float Timer;
    float Timer2;
    int levelIndex;
    private bool isPlayingWitchFireTile = false;
    private bool isPhase2BGMChanged = false;
    private float lastDamageTime = 0f;
    private const float DAMAGE_INTERVAL = 1f;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        weapons = GetComponentsInChildren<BossWeapon>();
        smash = GetComponentInChildren<Smash>(true);
        CC = FindObjectOfType<CameraControl>();
    }

    public void Bottle()
    {
        weapons[0].Shut(0, (int)(GameManager.Instance.maxHealth * 0.05f));
    }
    public void Crows()
    {
        weapons[0].Range(2, (int)(GameManager.Instance.maxHealth * 0.3f));
        // 효과음 길이만큼 대기
        Global.SoundManager.PlaySFX(SFXEnum.Witch_Crow_1);
        Global.SoundManager.PlaySFX(SFXEnum.Witch_Crow_2, delay: 0.1f);
    }
    void Update()
    {
        if (!GameManager.Instance.isLive) return;
        if (!isLive)
        {
            transform.Translate(0, -5 * Time.deltaTime, 0);
            ClearWitchAchievement();
            StageClearAsync().Forget();
            foreach (var weapon in weapons)
            {
                if (weapon.gameObject.activeSelf)
                {
                    weapon.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (BossManager.Instance.phase >= 2)
            {
                // 2페이즈 진입 시 BGM 변경 (한 번만)
                if (GameManager.Instance.CurStage == 12 && !isPhase2BGMChanged)
                {
                    Global.SoundManager.PlayMusic(BGMEnum.EchoesOfValhallaCombatVer);
                    isPhase2BGMChanged = true;
                }
                
                Timer += Time.deltaTime;
                if (Timer > 30f)
                {
                    Timer = 0f;
                    bossLevel[levelIndex++].SetActive(true);
                    levelIndex = Mathf.Min(bossLevel.Length - 1, levelIndex);
                }

                if (IsAnyBossLevelActive() && !isPlayingWitchFireTile)
                {
                    isPlayingWitchFireTile = true;
                    Global.SoundManager.PlaySFX(SFXEnum.Witch_FireTile_1);
                }
            }

            if (isPlayingWitchFireTile && !IsAnyBossLevelActive())
            {
                isPlayingWitchFireTile = false;
                Global.SoundManager.StopSFX(SFXEnum.Witch_FireTile_1);
            }

            Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0, 7.5f, 0), new Vector2(13, 4), 0, LayerMask.GetMask("Player"));

            if (BossManager.Instance.phase >= 1)
            {

                if (Timer2 > smashTime)
                {

                    Timer2 = 0f;
                    if (hit != null)
                    {
                        anim.SetBool("Hammer", true);
                        //GameManager.Instance.player.rigid.AddForce(new Vector2(0, -60), ForceMode2D.Impulse);
                    }
                    else
                    {
                        anim.SetBool("Smash", true);
                        //smash.gameObject.SetActive(true); Deleted
                    }

                }
                Timer2 += Time.deltaTime;
            }
        }
    }

    private void ClearWitchAchievement()
    {
        // First_Witch_Defeat 업적 클리어
        var achievementsManager = FindAnyObjectByType<AchievementsManager>();
        if (achievementsManager != null)
        {
            achievementsManager.AchivementTrueByID("First_Witch_Defeat");
        }

        // Witch 처치 카운트 증가 및 저장
        var userDataManager = Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (killSaveData.ContainsKey("Witch"))
            {
                killSaveData["Witch"]++;
            }
            else
            {
                killSaveData["Witch"] = 1;
            }
            userDataManager.Save();

            // 10번 처치 시 Witch_Slayer 업적 클리어
            if (killSaveData["Witch"] >= 10)
            {
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID("Witch_Slayer");
                }
            }
        }
    }

    async UniTaskVoid StageClearAsync()
    {
        GameManager.Instance.isInvincible = true;
        Global.SoundManager.StopBGMCompletely(); // 보스 클리어 시 BGM 정지
        await UniTask.Delay(3000); // 3초를 밀리초로 변환
        GameManager.Instance.StageClear();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - new Vector3(0, 7.5f, 0), new Vector2(13, 4));
    }
    void OnEnable()
    {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        sprite.sortingOrder = 1;
        // 일반 몬스터 체력 계산식(10 * Mathf.Pow(1.2f, stage))의 200배 (중간보스의 4배)
        maxHealth = 20 * Mathf.Pow(1.2f, GameManager.Instance.CurStage) * 200;
        health = maxHealth;
        GameManager.Instance.BossHealth = health;
        GameManager.Instance.maxBossHealth = maxHealth;
        
        // BGM 변경 플래그 리셋
        isPhase2BGMChanged = false;
        
        // 보스 스테이지용 카메라 사이즈 조정
        if (CC != null)
        {
            CC.SetBossCameraSize(11.5f);
        }
    }

    public void Hammer()
    {
        anim.SetBool("Hammer", false);
        anim.SetBool("Hamming", true);
        Invoke("StartShake", 0.2f);
    }

    public void StartShake()
    {
        CC.ShakeCamera();
    }
    public void AnimOff()
    {
        transform.GetChild(3).gameObject.SetActive(false);
        anim.SetBool("Hamming", false);
    }

    public void EarthQuakeOn()
    {
        Global.SoundManager.PlaySFX(SFXEnum.Witch_MagicCharge);
        transform.GetChild(3).gameObject.SetActive(true);
        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0, 7.5f, 0), new Vector2(13, 4), 0, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            Player player = hit.GetComponent<Player>();
            player.rigid.AddForce(new Vector2(0, -60), ForceMode2D.Impulse);
            player.Stopping();
            var damage = GameManager.Instance.maxHealth * 0.5f;
            GameManager.Instance.Health -= damage;
        }
    }

    public void TracePlayerOn()
    {
        transform.GetChild(2).GetChild(0).GetComponent<SmashUptoDown>().OnColor();

    }

    public void StopToTracing()
    {
        transform.GetChild(2).GetComponent<SmashFinish>().StopPositionToTarget();
    }

    public void SmashUptoDown()
    {
        anim.SetBool("Smash", false);
        anim.SetBool("Smashing", true);
        Transform child = transform.GetChild(2);
        child.GetChild(0).GetComponent<SmashUptoDown>().OffColor();
        child.gameObject.GetComponent<Animator>().SetTrigger("isSmash");
        Global.SoundManager.PlaySFX(SFXEnum.Witch_SpaceMove);

    }

    private void CalculateDamage(DamageData damageData)
    {
        float finalDamage = damageData.damage;
        bool isCritical = damageData.isCritical;

        finalDamage = finalDamage * (1 + GameManager.Instance.player.damageBonus);

        health -= finalDamage;
        GameManager.DamageTextPoolManager.SpawnDamageText(damageTextSpawnPoint.position, finalDamage, isCritical, false);

        if (GameManager.Instance.weaponController.isBonusDamage && damageData.isBonus)
        {
            float bonusDamage = finalDamage * GameManager.Instance.weaponController.bonusDamage;
            health -= bonusDamage;
            DelayedSpawnDamageText(transform.position, bonusDamage, 0.1f, true).Forget();
        }
    }

    private async UniTaskVoid DelayedSpawnDamageText(Vector3 position, float damage, float delay, bool isBonus = false)
    {
        await UniTask.Delay((int)(delay * 1000));
        GameManager.DamageTextPoolManager.SpawnDamageText(position, damage, false, isBonus);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet")) return;
        if (!isLive) return;

        if (collision.TryGetComponent(out Bullet bullet))
        {
            Global.SoundManager.PlayHitSFX(SFXEnum.Monster_Hit_1, isShootCooldown: false);
            CalculateDamage(bullet.CalculateDamage());
        }

        if (health <= 0)
        {
            isLive = false;
            coll.enabled = false;
            sprite.sortingOrder = -2;
            anim.SetBool("Dead", true);
            CC.ShakeCamera();
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Floor")) return;
        if (!isLive) return;
        if (!collision.TryGetComponent(out Bullet bullet)) return;

        if (Time.time >= lastDamageTime + DAMAGE_INTERVAL)
        {
            CalculateDamage(bullet.CalculateDamage());
            lastDamageTime = Time.time;

            if (health <= 0)
            {
                isLive = false;
                coll.enabled = false;
                sprite.sortingOrder = -2;
                anim.SetBool("Dead", true);
                CC.ShakeCamera();
            }
        }
    }

    private bool IsAnyBossLevelActive()
    {
        foreach (var level in bossLevel)
        {
            if (level.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}

