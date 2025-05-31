using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BulletWeapon : Weapon
{
    [SerializeField] protected float bulletSpawnInterval = 0.1f;

    public override void ExecuteAttack()
    {
        StartCoroutine(SpawnBullets());
    }
    
    public virtual IEnumerator SpawnBullets()
    {
        for (int i = 0; i < count; i++)
        {
            SpawnBullet();
            
            if (i < count - 1)
            {
                yield return new WaitForSeconds(bulletSpawnInterval);
            }
        }
    }
    
    public virtual void SpawnBullet()
    {
        // 기본 구현은 자식 클래스에서 오버라이드
    }
}
