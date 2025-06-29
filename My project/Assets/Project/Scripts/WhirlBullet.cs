using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;

public class WhirlBullet : Bullet
{
    float timer2;
    [Header("이동 방식")]
    public bool followPlayer = true; // Inspector에서 설정 가능한 토글

    CircleCollider2D coll;
    private float soundPlayInterval = 0.5f;  // 효과음 재생 주기
    private float soundTimer;  // 효과음 재생 타이머

    protected override void Awake() {
        base.Awake();
        coll = GetComponent<CircleCollider2D>();
        rb.isKinematic = followPlayer; // followPlayer가 true면 isKinematic true
    }

    public override void Init(float damage, int per, Vector3 dir, bool canStun = false,
    float duration = 0f, float criticalDamagePercent = 0f, float criticalChancePercent = 0f, float size = 0f)
    {
        base.Init(damage, per, dir, canStun, duration, criticalDamagePercent, criticalChancePercent, size);
        
        // 사운드 타이머 초기화
        soundTimer = soundPlayInterval;

        if (!followPlayer) {
            // 기존 방식대로 velocity 설정
            rb.velocity = dir;
        }
    }
    private void OnDisable() {
        Global.SoundManager.StopSFX();
    }

    void FixedUpdate() {
        timer2 += Time.deltaTime;
        soundTimer -= Time.deltaTime;  // 사운드 타이머 업데이트

        if (timer2 > 0.01f) {
            timer2 = 0f;
            Collider2D[] enemyColls = Physics2D.OverlapCircleAll(transform.position, coll.radius, LayerMask.NameToLayer("Enemy"));
            foreach (Collider2D enemyColl in enemyColls) {
                enemyColl.GetComponent<Enemy>().GetAddForce(transform.position);
            }
        }

        // 효과음 재생
        if (soundTimer <= 0) {
            Global.SoundManager.PlaySFX(SFXEnum.WindFloor);  // 효과음 재생
            soundTimer = soundPlayInterval;  // 타이머 리셋
        }
    }


}
