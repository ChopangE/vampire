using Cysharp.Threading.Tasks;
using Data;
using Manager.InGame;
using SO;
using UnityEngine;
public class WindWeapon : Weapon
{
    public override void ExecuteAttack()
    {
        float angleStep = 360f / count;  // count개 만큼 균등하게 나눈 각도
        float radius = 2f;  // 시작 위치의 반경

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i;
            // 원형으로 시작 위치 배치
            Vector3 startPos = transform.position + Quaternion.Euler(0, 0, angle) * Vector3.right * radius;
            
            Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
            bullet.position = startPos;
            // 각 총알은 같은 방향(GetPlayerDirection)으로 나선형 움직임
            bullet.GetComponent<WingWind>().Init(damage, -1, GetPlayerDirection(), false, 0f, criticalDamagePercent, criticalChancePercent);
        }
    }

    public override async UniTaskVoid Init()
    {
        base.Init().Forget();
        await UniTask.Yield();
        maxCooldown = 5f;
    }
}