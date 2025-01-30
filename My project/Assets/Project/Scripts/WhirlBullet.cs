using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlBullet : Bullet
{
    public float duration_;
    float timer;
    float timer2;
    [Header("이동 방식")]
    public bool followPlayer = true; // Inspector에서 설정 가능한 토글

    CircleCollider2D coll;
    protected override void Awake() {
        base.Awake();
        coll = GetComponent<CircleCollider2D>();
        rb.isKinematic = followPlayer; // followPlayer가 true면 isKinematic true
    }

    public override void Init(float damage, int per, Vector3 dir, bool canStun = false,
    float duration = 0f, float criticalDamagePercent = 0f, float criticalChancePercent = 0f)
    {
        this.damage = damage;
        this.criticalDamagePercent = criticalDamagePercent;
        this.criticalChancePercent = criticalChancePercent;
        this.per = per;
        this.canStun = canStun;
        this.duration = duration;

        if (!followPlayer) {
            // 기존 방식대로 velocity 설정
            rb.velocity = dir;
        }
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
                enemyColl.GetComponent<Enemy>().GetAddForce(transform.position);

            }

        }
    }


}
