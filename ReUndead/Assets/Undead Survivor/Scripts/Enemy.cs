using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }

    void FixedUpdate()
    {
        if (!isLive) return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextVec + rigid.position);
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate() {
        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable() {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        health = maxHealth;
    }

    public void Init(SpawnData data) {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Bullet")) return;

        health -= collision.GetComponent<Bullet>().damage;

        if(health> 0) {
            //Live
        }
        else {
            //Die
            Dead();
        }

    }

    void Dead() {
        gameObject.SetActive(false);
    }
}
