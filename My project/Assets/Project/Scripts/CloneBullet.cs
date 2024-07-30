using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneBullet : Bullet
{
    float timer;
    float curTime;
    void OnEnable() {
        Init2();
    }
    void Init2() {
        timer = 0f;
        curTime = 7f;
    }
    public override void Init(float damage, int per, Vector3 dir) {
        this.damage = damage;
        this.per = per;
        
       
       rb.velocity = dir * 15f;
       
    }
    void Update() {
        if(timer > curTime) {
            timer = 0f;
            gameObject.SetActive(false);
        }
    }


    public override void OnTriggerEnter2D(Collider2D collision) {
        if ((!collision.CompareTag("Enemy") && !collision.CompareTag("BossEnemy"))) {
            return;
        }
    }


}
