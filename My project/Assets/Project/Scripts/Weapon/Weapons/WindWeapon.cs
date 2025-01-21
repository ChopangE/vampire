using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
public class WindWeapon : Weapon
{
    public override void ExecuteAttack()
    {
        if (!player.scan.nearestTarget) return;

        Vector3 targetPos = player.scan.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;

        bullet.position = transform.position;
        bullet.GetComponent<Bullet>().Init(damage, -1, dir, criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
    }

    public override async UniTaskVoid Init(ItemData data)
    {
        base.Init(data).Forget();
        await UniTask.Yield();
        maxCooldown = 5f;
    }
}