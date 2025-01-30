using System.Collections;
using System.Collections.Generic;
using InGame.Data;
using Unity.VisualScripting;
using UnityEngine;

public class BowlingBullet : Bullet
{
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 충돌 지점에서 적의 위치를 빼서 밀어낼 방향 계산
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                // 5 타일 만큼의 힘으로 적을 밀어냄
                enemy.GetAddForce(pushDirection * 5f);
            }
        }
    }
}

