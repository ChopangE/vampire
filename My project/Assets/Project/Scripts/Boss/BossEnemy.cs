using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    Rigidbody2D target;
    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    
    void FixedUpdate() {
        if (!GameManager.Instance.isLive) return;

        if (!isLive) return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextVec + rigid.position);
        rigid.velocity = Vector2.zero;

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
        health = maxHealth;
    }
    public void GetDamage(float damage) {
        health -= damage;
        StartCoroutine(KnockBack());

        if (health < 0) {
            Dead();
        }
    }
    public void Init(int index) {
        anim.runtimeAnimatorController = animCon[index];
    }
    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Bullet")) return;

        health -= collision.GetComponent<Bullet>().damage;
        if (collision.CompareTag("Melee")) {
            Debug.Log("hihi");
            StartCoroutine(KnockBack());
        }

       if(health < 0) {
            Dead();

       }
    }
    void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Floor")) return;
        health -= collision.GetComponent<FloorWeapon>().damage;

        if (health <= 0) {
            Dead();
        }
    }
    void Dead() {
        isLive = false;
        coll.enabled = false;
        rigid.simulated = false;
        spriter.sortingOrder = 1;
        gameObject.SetActive(false);
    }
    IEnumerator KnockBack() {

        yield return null;  // 1물리 프레임 wait
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 dir = transform.position - playerPos;
        rigid.AddForce(dir.normalized * 5, ForceMode2D.Impulse);

    }
}
