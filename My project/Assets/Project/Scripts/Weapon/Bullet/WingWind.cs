using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingWind : Bullet
{
    Animator enemyAnim;
    Rigidbody2D enemyRb;

    public float addPower;
    private float spiralAngle;
    private float spiralRadius = 1f;
    [SerializeField] private float spiralSpeed = 5f;
    [SerializeField] private float expandSpeed = 2f;
    private Vector3 centerPosition;
    private Vector3 moveDirection;
    private bool isClockwise;

    public void ExitWind()
    {
        gameObject.SetActive(false);
    }

    public override void Init(float damage, int per, Vector3 dir, bool clockwise, float duration = 0, float criticalDamagePercent = 0, float criticalChancePercent = 0)
    {
        base.Init(damage, per, dir, false, duration, criticalDamagePercent, criticalChancePercent);
        rb.velocity = Vector2.zero;
        
        spiralAngle = 0f;
        centerPosition = transform.position;
        moveDirection = dir;
        isClockwise = clockwise;
        
        StartCoroutine(SpiralMovement());
    }

    private IEnumerator SpiralMovement()
    {
        while (gameObject.activeSelf)
        {
            // 회전 방향에 따라 각도 증가/감소
            float rotationDirection = isClockwise ? 1f : -1f;
            spiralAngle += rotationDirection * spiralSpeed * Time.deltaTime;
            float currentRadius = spiralRadius * (1 + expandSpeed * Mathf.Abs(spiralAngle));
            
            Vector3 offset = new Vector3(
                Mathf.Cos(spiralAngle) * currentRadius,
                Mathf.Sin(spiralAngle) * currentRadius,
                0
            );
            
            // 진행 방향에 맞춰 회전
            Vector3 rotatedOffset = Quaternion.FromToRotation(Vector3.right, moveDirection) * offset;
            
            // 중심점 이동
            centerPosition += moveDirection * spiralSpeed * Time.deltaTime;
            
            // 최종 위치 설정
            transform.position = centerPosition + rotatedOffset;
            
            yield return null;
        }
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
        enemyAnim.SetTrigger("Hit");
        Vector2 dir = (collision.transform.position - transform.position).normalized;
        if(enemyRb) enemyRb.AddForce(dir * addPower, ForceMode2D.Impulse);

    }
}
