using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;

public class SecretSmash : Bullet
{
    // 보스 타입별 데미지 비율 설정
    [Header("보스 타입별 데미지 비율")]
    [Tooltip("중간 보스에게 적용할 체력 감소 비율 (0.7 = 70%)")]
    [SerializeField] private float middleBossDamageRatio = 0.7f;
    
    [Tooltip("최종 보스에게 적용할 체력 감소 비율 (0.4 = 40%)")]
    [SerializeField] private float finalBossDamageRatio = 0.4f;
    private void OnEnable() {
        Init(0, -1, Vector3.zero, false, 1.05f, 0, 0);
        Global.SoundManager.PlaySFX(SFXEnum.UltimateSwing);
        var cc = FindObjectOfType<CameraControl>();
        cc.ShakeCamera();
    }
    public void StopShake() {
        var cc = FindObjectOfType<CameraControl>();
        cc.StopCameraShake();
    }
    public override void Init(float damage, int per, Vector3 dir, bool canStun = false,
    float duration = 0f, float criticalDamagePercent = 0f, float criticalChancePercent = 0f)
    {
        base.Init(damage, per, dir, canStun, duration, criticalDamagePercent, criticalChancePercent);

        // 플레이어 방향에 따라 회전 설정
        if(GameManager.Instance.player != null)
        {
            bool isFacingRight = GameManager.Instance.player.IsFacingRight;
            
            // 스프라이트 방향 설정
            if(TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer.flipX = !isFacingRight;
            }
            
            // 또는 오브젝트 자체를 회전시키는 방법
            // transform.rotation = Quaternion.Euler(0, isFacingLeft ? 180 : 0, 0);
        }
    }
    
    // 적 유형에 따라 다른 데미지 적용
    public void ApplySpecialDamage(GameObject target)
    {
        // 중간 보스인 경우
        if (target.TryGetComponent(out MiddleBoss middleBoss))
        {
            // 현재 체력의 70%를 감소
            float damageAmount = middleBoss.health * middleBossDamageRatio;
            middleBoss.health -= damageAmount;
            
            // 데미지 텍스트 표시
            GameManager.DamageTextPoolManager.SpawnDamageText(
                middleBoss.transform.position, 
                damageAmount, 
                true, // 크리티컬로 표시
                false
            );
            
            Debug.Log($"중간 보스에게 {damageAmount}의 데미지 적용 (체력의 {middleBossDamageRatio * 100}%)");
        }
        // 최종 보스인 경우
        else if (target.TryGetComponent(out Boss boss))
        {
            // 현재 체력의 40%를 감소
            float damageAmount = boss.health * finalBossDamageRatio;
            boss.health -= damageAmount;
            
            // 데미지 텍스트 표시
            GameManager.DamageTextPoolManager.SpawnDamageText(
                boss.transform.position, 
                damageAmount, 
                true, // 크리티컬로 표시
                false
            );
            
            Debug.Log($"최종 보스에게 {damageAmount}의 데미지 적용 (체력의 {finalBossDamageRatio * 100}%)");
        }
        // 일반 몹이나 엘리트 몹은 Enemy.cs에서 처리됨 (즉사)
    }
    
    // 충돌 시 특수 효과 적용
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // 적과 충돌한 경우
        if (collision.CompareTag("Enemy"))
        {
            // 중간 보스나 최종 보스인 경우 특수 데미지 적용
            if (collision.TryGetComponent(out MiddleBoss _) || 
                collision.TryGetComponent(out Boss _))
            {
                ApplySpecialDamage(collision.gameObject);
            }
            // 일반 몹과 엘리트 몹은 Enemy.cs의 OnTriggerEnter2D에서 처리됨
        }
    }
}
