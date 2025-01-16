using UnityEngine;
using Cysharp.Threading.Tasks;
public class SuperAmulet : Weapon
{
    public override async UniTaskVoid Init(ItemData data)
    {
        base.Init(data).Forget();
        await UniTask.Yield();
        maxCooldown = 5f;
    }
    public override void ExecuteAttack()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.GetChild(0).GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
    }
}
