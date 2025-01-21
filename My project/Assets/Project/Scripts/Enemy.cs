using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using InGame;
using InGame.Data;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
    int coinNum;
    protected bool isLive;
    int level;
    protected float timer;
    bool isBack;

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

    protected override void Awake()
    {
        base.Awake();
        coinNum = 4;
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        weaponController = GameManager.Instance.weaponController;
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isLive) return;
        if (!isLive) return;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;
        if (_isStunned) return; // 스턴 상태일 때는 이동하지 않음

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextVec + rigid.position);
        //transform.Translate(nextVec);
        //rigid.velocity = Vector2.zero;


    }

    void LateUpdate()
    {
        if (!GameManager.Instance.isLive) return;

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
        isBack = false;
        isStunned = false; // 활성화될 때 스턴 해제
    }

    public void Init(SpawnData data)
    {
        base.OnEnable();
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        level = data.spriteType;
        _enemyType = EnemyType.Normal;
    }
    public void InitElite(SpawnData data)
    {
        anim.runtimeAnimatorController = eliteanimCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        level = data.spriteType;
        _enemyType = EnemyType.Elite;

    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet")) return;
        if (collision.GetComponent<Bullet>())
        {
            CalculateDamage(collision.GetComponent<Bullet>().CalculateDamage());

            if (collision.GetComponent<WhirlBullet>())
            {
                targetVec = collision.GetComponent<Rigidbody2D>().position;
                if (gameObject.activeSelf) StartCoroutine(KnockBack());

            }
        }

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

    private void CalculateDamage(DamageData damageData)
    {
        health -= damageData.damage;
        GameManager.DamageTextPoolManager.SpawnDamageText(transform.position, damageData.damage, damageData.isCritical, false);
        if (weaponController.isBonusDamage)
        {
            if(damageData.isBonus)
            {
                float bonusDamage = damageData.damage * weaponController.bonusDamage;
                health -= bonusDamage;
                DelayedSpawnDamageText(transform.position, bonusDamage, 0.1f, true).Forget();
            }
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
        if (!collision.CompareTag("Floor")) return;
        health -= collision.GetComponent<WhirlBullet>().damage / 10.0f;
        //Vector3 dir = collision.transform.position - transform.position;
        //rigid.AddForce(dir.normalized * 4, ForceMode2D.Impulse);
        //StartCoroutine(KnockBack(collision.transform.position));
        if (health <= 0)
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            //anim.SetBool("Dead", true);
            Dead();
        }
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
            Global.ExpManager.SpawnExpItem(level / 4 + 1, transform.position);
        }
        else
        {
            Global.ExpManager.SpawnExpItem(0, transform.position);
            GameManager.Instance.ShowLevelUp();

            DropItem dropItem = Resources.Load<DropItem>("Prefabs/DropItem/GoldBarCoinGold");
            Vector3 dropPosition = dropItemSpawnPoints != null && dropItemSpawnPoints.Length > 0 
                ? dropItemSpawnPoints[0].position 
                : transform.position;
            GameManager.DropItemPoolManager.SpawnDropItem(dropItem, dropPosition);

            DropItem dropItem2 = Resources.Load<DropItem>("Prefabs/DropItem/SpecialItem/LevelUpScroll");
            Vector3 dropPosition2 = dropItemSpawnPoints != null && dropItemSpawnPoints.Length > 1 
                ? dropItemSpawnPoints[1].position 
                : transform.position;
            GameManager.DropItemPoolManager.SpawnDropItem(dropItem2, dropPosition2);
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
