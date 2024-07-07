using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    int coinNum;
    bool isLive;
    int level;
    float timer;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;
    void Awake()
    {
        coinNum = 4;
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isLive) return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;
        
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextVec + rigid.position);
        rigid.velocity = Vector2.zero;

        timer += Time.fixedDeltaTime;
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
    }

    public void Init(SpawnData data) {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        level = data.spriteType;
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Bullet")) return;

        health -= collision.GetComponent<Bullet>().damage;
        //StartCoroutine(KnockBack());

        if(health> 0) {
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
        health -= collision.GetComponent<FloorWeapon>().damage;

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

        yield return wait;  // 1물리 프레임 wait
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 dir = transform.position - playerPos;
        rigid.AddForce(dir.normalized * 5, ForceMode2D.Impulse);

    }

    void Dead() {
        GameObject coin = GameManager.Instance.pool.Get(coinNum);
        coin.transform.position = transform.position;
        coin.transform.rotation = Quaternion.identity;
        Coin cc = coin.GetComponent<Coin>();
        cc.sprite.sprite = cc.sprites[level];
        cc.exp = level + 1;                               //경험치 조절 여기서 가능
        GameManager.Instance.kill++;
        gameObject.SetActive(false);
    }
}
