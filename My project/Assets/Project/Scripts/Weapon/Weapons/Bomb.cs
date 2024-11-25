using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Weapon
{
    public override void Init(ItemData data)
    {
        base.Init(data);
        maxCooldown = 3f;
    }
    public override void ExecuteAttack()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        bullet.rotation = Quaternion.identity;
        bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero);
    }
}
