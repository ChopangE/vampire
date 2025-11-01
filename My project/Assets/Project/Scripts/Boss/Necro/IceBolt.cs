using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using Unity.VisualScripting;
using UnityEngine;

public class IceBolt : MiddleBossBullet
{
    Vector3 playerPos;
    Rigidbody2D rb;
    float addPower;

    Vector3 startScale;
    
    protected override void Init() {
        base.Init();
        rb = GetComponent<Rigidbody2D>();
        transform.position = mBoss.transform.position;
        playerPos = player.transform.position;
        Vector3 dir = (playerPos - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        startScale = new Vector3(3,3,3);
        addPower = 8.0f;
        rb.AddForce(dir * addPower, ForceMode2D.Impulse);
        StartCoroutine(GraduallyDescending());
        Global.SoundManager.PlaySFX(SFXEnum.IceBolt);
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
        // 3초 동안 30% 슬로우 적용
        GameManager.Instance.player.ApplySlow(3f, 0.3f);
    }

    IEnumerator GraduallyDescending() {
        float startSize = 1f;
        float endSize = 0f;
        float timer = 0f;
        float duration = 2f;
        
        while(timer < duration) {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(startSize, endSize, timer/duration);
            transform.localScale = startScale * scale;
            yield return null;
        }
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
}
