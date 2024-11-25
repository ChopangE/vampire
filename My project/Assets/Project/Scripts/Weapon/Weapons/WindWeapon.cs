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
        bullet.GetComponent<Bullet>().Init(damage, 50, dir);
    }

    public override void Init(ItemData data)
    {
        base.Init(data);
        maxCooldown = 5f;
    }
}