using System.Collections;
using System.Collections.Generic;
using InGame;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Enemy : MonoBehaviour {
    enum EnemyType
    {
        Elite, Normal,
    }

    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public RuntimeAnimatorController[] eliteanimCon;
    public Rigidbody2D target;

    private EnemyType _enemyType = EnemyType.Normal;
    int coinNum;
    bool isLive;
    int level;
    float timer;
    bool isBack;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;
    Vector2 targetVec;
    void Awake() {
        coinNum = 4;
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate() {
        if (!GameManager.Instance.isLive) return;
        if (!isLive) return;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextVec + rigid.position);
        //transform.Translate(nextVec);
        //rigid.velocity = Vector2.zero;
        
        
    }
  
    void LateUpdate() {
        if (!GameManager.Instance.isLive) return;

        if (isLive) {
            spriter.flipX = target.position.x < rigid.position.x;
        }
    }

    void OnEnable() {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true; 
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
        isBack = false;
    }

    public void Init(SpawnData data) {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        level = data.spriteType;
        _enemyType = EnemyType.Normal;
    }
    public void InitElite(SpawnData data) {
        anim.runtimeAnimatorController = eliteanimCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        level = data.spriteType;
        _enemyType = EnemyType.Elite;

    }
    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Bullet") && !collision.CompareTag("Floor")) return;
        if (collision.GetComponent<Bullet>()) {
            health -= collision.GetComponent<Bullet>().damage;
            if (collision.GetComponent<WhirlBullet>()) {
                targetVec = collision.GetComponent<Rigidbody2D>().position;
                if(gameObject.activeSelf) StartCoroutine(KnockBack());

            }
        }
        
        if (health> 0) {
            anim.SetTrigger("Hit");

        }
        else {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            //anim.SetBool("Dead", true);
            Dead();
        }
    }
    void OnTriggerExit2D(Collider2D collision) {
        if(collision.CompareTag("Circle")) {
            Vector2 dir = targetVec - rigid.position;
            anim.SetTrigger("Hit");
            rigid.AddForce(dir.normalized * 7, ForceMode2D.Impulse);

        }
    }
    public void GetDamage(float damage) {
        health -= damage;
        //StartCoroutine(KnockBack());
        if (health > 0) {
            anim.SetTrigger("Hit");
        } else {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            //anim.SetBool("Dead", true);
            Dead();
        }
    }
    void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Floor")) return;
        health -= collision.GetComponent<WhirlBullet>().damage / 10.0f;
        //Vector3 dir = collision.transform.position - transform.position;
        //rigid.AddForce(dir.normalized * 4, ForceMode2D.Impulse);
        //StartCoroutine(KnockBack(collision.transform.position));
        if(health <=0) {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            //anim.SetBool("Dead", true);
            Dead();
        }
    }
    IEnumerator KnockBack() {
        Vector2 dir = targetVec - rigid.position;
        yield return wait;          // 1물리 프레임 wait
        rigid.AddForce(dir.normalized * 5, ForceMode2D.Impulse);
        yield return wait;          // 1물리 프레임 wait
    }

    void Dead() {
        // GameObject coin = GameManager.Instance.pool.Get(coinNum);
        // coin.transform.position = transform.position;
        // coin.transform.rotation = Quaternion.identity;
        // Coin cc = coin.GetComponent<Coin>();
        // cc.sprite.sprite = cc.sprites[Mathf.Min((level / 4),cc.sprites.Length-1)];
        // cc.exp = level + 1;                               //경험치 조절 여기서 가능
        if(_enemyType == EnemyType.Normal)
            Global.ExpManager.SpawnExpItem(level/4 + 1, transform);
        else
        {
            Global.ExpManager.SpawnExpItem(0, transform);
            GameManager.Instance.ShowLevelUp();
            DropItemPoolManager poolManager = FindObjectOfType<DropItemPoolManager>();
            DropItem dropItem = Resources.Load<DropItem>("Prefabs/DropItem/GoldBarCoinGold");
            poolManager.SpawnDropItem(dropItem,transform.position);
        }
        GameManager.Instance.kill++;
        gameObject.SetActive(false);
    }
    public void GetAddForce(Vector3 pos) {
        Vector2 enemyToCircle = (pos - transform.position).normalized;
        rigid.AddForce(enemyToCircle * 10, ForceMode2D.Impulse);
    }
}
