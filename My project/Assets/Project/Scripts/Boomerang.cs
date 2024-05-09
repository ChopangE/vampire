using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    Rigidbody2D rb;
    public Player player;
    float timer = 0f;
    bool isShoot = false;
    bool isComing = false;
    Vector2 pp;
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (!GameManager.Instance.isLive) return;
        Vector2 curVec = player.inputVec.normalized;
        transform.Rotate(0, 0, 3000f * Time.fixedDeltaTime);
        if (!isShoot) {
            rb.MovePosition(player.rigid.position);
        }

        if (isShoot) {
            timer += Time.fixedDeltaTime;
        }

        if (Input.GetKey(KeyCode.Space) && !isShoot && !isComing) {
            pp = curVec;
            if(pp.magnitude < 0.1f) {
                pp = player.GetComponent<SpriteRenderer>().flipX ? new Vector2(-1f, 0) : new Vector2(1f, 0);
            }
            rb.AddForce(pp * 8f, ForceMode2D.Impulse);
            isShoot = true;

        }

        if (timer >= 0.5f && isShoot) {
            rb.AddForce(-pp * 4f, ForceMode2D.Impulse);
            timer = 0f;
        }

        if (rb.velocity.magnitude <= 0.3f && isShoot) {
            isComing = true;
            isShoot = false;
            rb.velocity = Vector2.zero;
        }

        if (isComing) {
            Vector2 dir = player.rigid.position - rb.position;
            dir = dir.normalized;
            rb.MovePosition(rb.position + dir * Time.fixedDeltaTime * 10f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;
        if (isComing) { 
            isShoot = false;
            isComing = false;
            timer = 0f;
        }
    }
    void OnEnable() {
        isShoot = false;
        isComing = false;
    }
}
