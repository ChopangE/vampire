using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlBullet : Bullet
{
    public float duration_;
    float timer;
    float timer2;

    CircleCollider2D coll;
    void Awake() {
        coll = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        timer += Time.deltaTime;
        if(timer > duration_) {
            timer = 0f;
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate() {
        timer2 += Time.deltaTime;
        if (timer2 > 0.2f) {
            timer2 = 0f;
            Collider2D[] enemyColls = Physics2D.OverlapCircleAll(transform.position, coll.radius, LayerMask.NameToLayer("Enemy"));
            foreach (Collider2D enemyColl in enemyColls) {
                Debug.Log("hi");
                //enemyColl.GetComponent<Enemy>().GetAddForce(transform.position);

            }

        }
    }

    public override void OnTriggerEnter2D(Collider2D collision) {}


    

}
