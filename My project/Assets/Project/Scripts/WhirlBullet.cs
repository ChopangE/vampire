using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlBullet : Bullet
{
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
        base.Init(damage, per, dir, canStun, duration, criticalDamagePercent, criticalChancePercent);

        if (!followPlayer) {
            // 기존 방식대로 velocity 설정
            rb.velocity = dir;
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
