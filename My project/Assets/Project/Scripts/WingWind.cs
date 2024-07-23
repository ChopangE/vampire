using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingWind : Bullet
{
    Animator enemyAnim;
    Rigidbody2D enemyRb;

    public float addPower;
    public void ExitWind()
    {
        gameObject.SetActive(false);
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if ((!collision.CompareTag("Enemy") && !collision.CompareTag("BossEnemy")))
        {
            return;
        }
        if (!collision.gameObject.activeSelf) return;
        enemyAnim = collision.GetComponent<Animator>();
        enemyRb = collision.GetComponent<Rigidbody2D>();
        enemyAnim.SetTrigger("hit");
        Vector2 dir = (collision.transform.position - transform.position).normalized;
        if(enemyRb) enemyRb.AddForce(dir * addPower, ForceMode2D.Impulse);

    }
}
