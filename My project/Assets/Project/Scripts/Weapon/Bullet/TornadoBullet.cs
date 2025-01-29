using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoBullet : Bullet
{
    Animator enemyAnim;
    Rigidbody2D enemyRb;
    
    public float addPower = 5f;  // 넉백 파워
    public float moveSpeed = 3f;  // 토네이도 이동 속도
    public float directionChangeTime = 2f;  // 방향 전환 주기
    private float directionTimer;
    private Vector3 currentDirection;
    private float aliveTime;

    public override void Init(float damage, int per, Vector3 dir, bool clockwise, float duration = 0, float criticalDamagePercent = 0, float criticalChancePercent = 0)
    {
        base.Init(damage, per, dir, false, duration, criticalDamagePercent, criticalChancePercent);
        rb.velocity = Vector2.zero;
        

        // 초기 랜덤 방향 설정
        SetRandomDirection();
        directionTimer = directionChangeTime;
        aliveTime = 0f;
        
        StartCoroutine(MovementCoroutine());
    }

    private void SetRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        currentDirection = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0).normalized;
    }

    private IEnumerator MovementCoroutine()
    {
        while (gameObject.activeSelf && aliveTime < duration)
        {
            // 시간 업데이트
            aliveTime += Time.deltaTime;
            directionTimer -= Time.deltaTime;

            // 방향 전환 시간이 되면 새로운 랜덤 방향 설정
            if (directionTimer <= 0)
            {
                SetRandomDirection();
                directionTimer = directionChangeTime;
            }

            // 화면 경계 체크 및 방향 전환
            CheckScreenBounds();
            // 이동
            transform.position += currentDirection * moveSpeed * Time.deltaTime;

            yield return null;
        }

        // 지속시간 종료
        gameObject.SetActive(false);
    }

    private void CheckScreenBounds()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector3 pos = transform.position;
        bool needsDirectionChange = false;

        if (Mathf.Abs(pos.x) > screenBounds.x - 1f)
        {
            currentDirection.x *= -1;
            needsDirectionChange = true;
        }
        if (Mathf.Abs(pos.y) > screenBounds.y - 1f)
        {
            currentDirection.y *= -1;
            needsDirectionChange = true;
        }

        if (needsDirectionChange)
        {
            directionTimer = directionChangeTime;
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") && !collision.CompareTag("BossEnemy"))
        {
            return;
        }
        
        if (!collision.gameObject.activeSelf) return;
        
        enemyAnim = collision.GetComponent<Animator>();
        enemyRb = collision.GetComponent<Rigidbody2D>();
        
        if (enemyAnim) enemyAnim.SetTrigger("Hit");
        
        // 넉백 방향 계산 및 적용
        Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
        if (enemyRb) enemyRb.AddForce(knockbackDir * addPower, ForceMode2D.Impulse);
    }
}
