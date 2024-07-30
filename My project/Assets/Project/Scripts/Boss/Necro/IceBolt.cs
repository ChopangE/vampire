using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IceBolt : MiddleBossBullet
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
        transform.localScale = Vector3.one;
        addPower = 8.0f;
        rb.AddForce(dir * addPower, ForceMode2D.Impulse);
        StartCoroutine(GraduallyDescending());

    }
    protected override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Player") || collision.CompareTag("Bound")) {
            gameObject.SetActive(false);
        }
    }

    IEnumerator GraduallyDescending() {
        float startSize = 1f;
        float endSize = 0f;
        float timer = 0f;
        float duration = 2f;
        
        while(timer < duration) {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(startSize, endSize, timer/duration);
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
}
