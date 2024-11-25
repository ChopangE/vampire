using UnityEngine;

public class HGDClone : Weapon
{
    public override void Init(ItemData data)
    {
        base.Init(data);
        maxCooldown = 7f;
    }
    public override void ExecuteAttack()
    {
        Vector3 dir = GetPlayerDirection();
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        bullet.GetComponent<Bullet>().Init(damage, 100, dir);
    }
}
