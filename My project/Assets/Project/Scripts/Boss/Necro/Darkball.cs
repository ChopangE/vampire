using System.Collections;
using System.Collections.Generic;
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
    }
    protected override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Player") || collision.CompareTag("Bound")) {
            gameObject.SetActive(false);
        }
        
    }
}
