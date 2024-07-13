using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;
    public Rigidbody2D rb;
    float duration;
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir) {
        this.damage = damage;
        this.per = per;
        if (per <= -100)
        {
            duration = Random.Range(3f, 5f);
            StartCoroutine(Stop());
        }

        if (per > -1) {
            rb.velocity = dir * 7f;
        }
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision) {
        if ((!collision.CompareTag("Enemy")&& ! collision.CompareTag("BossEnemy")) || per == -1) {
            return;
        }
        
        per--;

        if(per == -1) {
            rb.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}
