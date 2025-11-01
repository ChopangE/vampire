using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Bomb : Weapon
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override void ExecuteAttack()
    {
        // Calculate damage: 8 * (level + 1), where level is 0-based
        int scaledDamage = 8 * level;
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        bullet.rotation = Quaternion.identity;
        bullet.GetComponent<Trap>().Init(scaledDamage, -100, Vector3.zero, criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
    }
}
