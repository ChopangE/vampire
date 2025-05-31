using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Data;
using Manager;

public class Stick : BulletWeapon
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override void SpawnBullet()
    {
        Global.SoundManager.PlaySFX(SFXEnum.StaffSkill);
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        float dir = player.GetComponent<SpriteRenderer>().flipX ? -1f : 1f;
        
        bullet.position = transform.position;
        bullet.localScale = new Vector3(dir, 1, 1);
    }
}
