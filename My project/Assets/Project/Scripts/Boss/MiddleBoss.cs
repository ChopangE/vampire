using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class MiddleBoss : Enemy {

    public bool isDoing;

    Transform playerPos;

    void Start() {
        Init();
    }

    public virtual void Init() {
        playerPos = GameManager.Instance.player.GetComponent<Transform>();
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();

        speed = 5f;
        maxHealth = 500 * (GameManager.Instance.CurStage + 1);
        health = 100f;

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
    public override void Dead()
    {
        MiddleBossDead();
        gameObject.SetActive(false);
    }

    protected virtual void MiddleBossDead() {
        GameManager.Instance.StageClear();
    }

    public void SetDoing() {
        isDoing = true;
    }
    public void EndDoing() {
        isDoing = false;
    }
}
