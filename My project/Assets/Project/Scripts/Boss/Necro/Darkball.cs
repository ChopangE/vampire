using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;

public class Darkball : MiddleBossBullet
{

    Vector3 playerPos;
    Rigidbody2D rb;
    float addPower;

    protected override void Init() {
        base.Init();
        rb = GetComponent<Rigidbody2D>();

        transform.position = mBoss.transform.position;
        playerPos = player.transform.position;
        Vector3 dir = (playerPos - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        addPower = 7.0f;
        rb.AddForce(dir * addPower, ForceMode2D.Impulse);
        Global.SoundManager.PlaySFX(SFXEnum.DarkFireBall);
    }
    protected override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Player") || collision.CompareTag("Bound")) {
            gameObject.SetActive(false);
        }
    }
    
    protected override void Damaging() {
        var damage = GameManager.Instance.maxHealth * 0.3f;
        GameManager.Instance.Health -= damage;
        // 2초 동안 화상 효과 적용 (1초마다 최대체력의 5% 데미지)
        GameManager.Instance.player.ApplyBurn(2f, 0.025f);
    }
}
