using Cysharp.Threading.Tasks;
using Data;
using Manager;
using Manager.InGame;
using SO;
using UnityEngine;
public class WindWeapon : Weapon
{
    [SerializeField] private SFXEnum sfxEnum;
    public override void ExecuteAttack()
    {
        float angleStep = 360f / count;  // count 기반으로 각도 간격 계산
        float startDistance = 1.5f;
        Global.SoundManager.PlaySFX(sfxEnum);
        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep;  // 현재 돌풍의 각도
            
            // 각도를 라디안으로 변환하여 방향 벡터 계산
            Vector3 direction = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0
            );
            
            // 시계방향 회전 설정 (서쪽 방향(180도)에 가까운 경우 반시계 방향으로)
            bool clockwise = !(angle > 135f && angle < 225f);
            
            Vector3 startPos = transform.position + direction * startDistance;
            
            Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
            bullet.position = startPos;
            bullet.GetComponent<WingWind>().Init(damage, -1, direction, clockwise, 0f, criticalDamagePercent, criticalChancePercent, size);
        }
    }

    public override async UniTask Init()
    {
        await base.Init();
    }
}