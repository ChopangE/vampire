using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class MiddleBoss : MonoBehaviour {

    protected float speed;
    protected float health;
    public bool isDoing;

    protected Animator anim;
    Transform playerPos;
    SpriteRenderer spriter;
    Rigidbody2D rigid;
    Rigidbody2D target;

    bool isLive;

    void Start() {
        Init();
    }

    public virtual void Init() {
        playerPos = GameManager.Instance.player.GetComponent<Transform>();
        spriter = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        speed = 5f;
        health = 100f;

        isLive = true;
        isDoing = false;
        
    }

    // Update is called once per frame
    protected virtual void Update() {
        if(health < 0.0f) {
            MiddleBossDead();
        }
    }
    void FixedUpdate() {
        if (isDoing) {
            rigid.velocity = Vector2.zero;
            return;
        }
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextVec + rigid.position);
    }
    void LateUpdate() {
        if (!GameManager.Instance.isLive) return;

        if (isLive) {
            spriter.flipX = target.position.x < rigid.position.x;
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Bullet")) return;
        Bullet bullet;
        if (bullet = collision.GetComponent<Bullet>()) {
            health -= bullet.damage;
        }
    }

    protected virtual void MiddleBossDead() {
        
    }

    public void SetDoing() {
        isDoing = true;
    }
    public void EndDoing() {
        isDoing = false;
    }
}
