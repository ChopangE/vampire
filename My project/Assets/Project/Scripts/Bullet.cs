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
    public bool canStun;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Init(float damage, int per, Vector3 dir, bool canStun = false, float duration = 0f)
    {
        this.damage = damage;
        this.per = per;
        this.canStun = canStun;
        if (duration == 0)
        {
            if (per <= -100)
            {
                duration = Random.Range(3f, 5f);
                StartCoroutine(Stop());
            }
        }

        if (per > -1)
        {
            rb.velocity = dir * 7f;
        }
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if ((!collision.CompareTag("Enemy") && !collision.CompareTag("BossEnemy")) || per == -1)
        {
            return;
        }

        if (canStun)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                StartCoroutine(ApplyStun(enemy));
            }
        }

        per--;

        if (per == -1)
        {
            rb.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    IEnumerator ApplyStun(Enemy enemy)
    {
        enemy.isStunned = true;
        yield return new WaitForSeconds(1f);
        enemy.isStunned = false;
    }
}
