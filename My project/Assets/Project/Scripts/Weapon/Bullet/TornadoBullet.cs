using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
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
    private float soundPlayInterval = 0.5f;  // 효과음 재생 주기
    private float soundTimer;  // 효과음 재생 타이머
    private bool isChangingDirection = false;  // 방향 전환 중인지 확인
    private float boundaryCheckCooldown = 1f;  // 경계 체크 쿨다운 시간
    private float boundaryCheckTimer = 0f;     // 경계 체크 타이머

    public override void Init(float damage, int per, Vector3 dir, bool clockwise, float duration = 0, float criticalDamagePercent = 0, float criticalChancePercent = 0)
    {
        base.Init(damage, per, dir, false, duration, criticalDamagePercent, criticalChancePercent);
        rb.velocity = Vector2.zero;
        
        // 초기 랜덤 방향 설정
        SetRandomDirection();
        directionTimer = directionChangeTime;
        aliveTime = 0f;
        soundPlayInterval = Global.SoundManager.GetSFXClipLength(SFXEnum.Tornado);
        soundTimer = 0;
        
        StartCoroutine(MovementCoroutine());
    }
    private void OnDisable() {
        Global.SoundManager.StopSFX();
    }

    private void SetRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        currentDirection = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0);
        
        // 방향 벡터가 0이 되지 않도록 보장
        if (Mathf.Approximately(currentDirection.magnitude, 0f))
        {
            currentDirection = Vector3.right;
        }
        
        currentDirection = currentDirection.normalized;
        // Debug.Log($"토네이도 속도: {moveSpeed}, 방향: {currentDirection}, 벡터 크기: {currentDirection.magnitude}");
    }

    private IEnumerator MovementCoroutine()
    {
        while (gameObject.activeSelf && aliveTime < duration)
        {
            // 시간 업데이트
            aliveTime += Time.deltaTime;
            directionTimer -= Time.deltaTime;
            soundTimer -= Time.deltaTime;  // 사운드 타이머 업데이트

            // 방향 전환 시간이 되면 새로운 랜덤 방향 설정
            if (directionTimer <= 0)
            {
                SetRandomDirection();
                directionTimer = directionChangeTime;
            }

            // 효과음 재생
            if (soundTimer <= 0)
            {
                Global.SoundManager.PlaySFX(SFXEnum.Tornado);  // 효과음 재생
                soundTimer = soundPlayInterval;  // 타이머 리셋
            }

            // 화면 경계 체크 및 방향 전환
            CheckScreenBounds();
            
            // 이동 - 정규화된 방향 벡터 사용
            Vector3 movement = currentDirection.normalized * moveSpeed * Time.deltaTime;
            transform.position += movement;

            yield return null;
        }

        // 지속시간 종료
        gameObject.SetActive(false);
    }

    private void CheckScreenBounds()
    {
        // 방향 전환 쿨다운 중이면 체크 스킵
        if (isChangingDirection)
        {
            boundaryCheckTimer -= Time.deltaTime;
            if (boundaryCheckTimer <= 0)
            {
                isChangingDirection = false;
            }
            return;
        }

        // 플레이어 위치 기준으로 7x7 영역의 경계 설정
        Vector3 playerPos = GameManager.Instance.player.transform.position; // 현재 위치 기준
        float minX = playerPos.x - 7f;
        float maxX = playerPos.x + 7f;
        float minY = playerPos.y - 7f;
        float maxY = playerPos.y + 7f;

        Vector3 pos = transform.position;
        bool needsDirectionChange = false;
        Vector3 newDirection = currentDirection;

        // X축 경계 체크
        if (pos.x < minX || pos.x > maxX)
        {
            newDirection.x = -currentDirection.x;
            needsDirectionChange = true;
        }

        // Y축 경계 체크
        if (pos.y < minY || pos.y > maxY)
        {
            newDirection.y = -currentDirection.y;
            needsDirectionChange = true;
        }

        // 방향 전환이 필요한 경우
        if (needsDirectionChange)
        {
            currentDirection = newDirection.normalized;
            isChangingDirection = true;
            boundaryCheckTimer = boundaryCheckCooldown;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
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
