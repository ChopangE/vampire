using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
public class Tornado : Weapon
{
    public override async UniTaskVoid Init()
    {
        base.Init().Forget();
        await UniTask.Yield();
    }
    public override void ExecuteAttack()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        bullet.rotation = Quaternion.identity;
        bullet.GetComponent<Bullet>().Init(
            damage,
            -1,
            Vector3.zero,
            criticalChancePercent: criticalChancePercent,
            criticalDamagePercent: criticalDamagePercent,
            duration: duration
        );
    }
}