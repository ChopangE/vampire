using UnityEngine;

public class Amulet : Weapon
{
    public override void Init(ItemData data)
    {
        base.Init(data);
        maxCooldown = 5f;
    }
    public override void ExecuteAttack()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.GetChild(0).GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
    }
}
