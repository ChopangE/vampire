using UnityEngine;
using Cysharp.Threading.Tasks;
public class Amulet : Weapon
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override void ExecuteAttack()
    {
        // 랜덤한 각도 생성 (0~360도)
        float randomAngle = Random.Range(0f, 360f);
        Vector3 dir = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;

        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.GetChild(0).GetComponent<Bullet>().Init(damage, -1, dir, 
        criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
    }
}
