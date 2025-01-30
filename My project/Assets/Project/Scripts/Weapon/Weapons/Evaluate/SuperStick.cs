using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SuperStick : Weapon
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override async void ExecuteAttack()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        float dir = player.GetComponent<SpriteRenderer>().flipX ? -1f : 1f;
        
        bullet.position = transform.position;
        bullet.localScale = new Vector3(dir, 1, 1);
        await UniTask.Delay(500);
        bullet.gameObject.SetActive(false);
    }
}
