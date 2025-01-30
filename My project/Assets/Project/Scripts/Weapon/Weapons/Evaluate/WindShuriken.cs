using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class WindShuriken : Weapon
{
    public override async UniTask Init()
    {
        await base.Init();
    }
    public override void ExecuteAttack()
    {
        Transform eliteOrBoss = player.scan.GetNearstEliteOrBoss();
        if (eliteOrBoss != null)
        {
            // 보스나 엘리트를 향해 발사
            Vector3 targetPos = eliteOrBoss.position;
            Vector3 dir = (targetPos - transform.position).normalized;
            
            Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            var bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Init(damage, -1, dir, 
            criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
            bulletScript.rb.velocity = dir * 7f;
        }
        else
        {
            // 보스나 엘리트가 없으면 랜덤 방향으로 발사
            float randomAngle = Random.Range(0f, 360f);
            Vector3 dir = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;

            Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            var bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Init(damage, -1, dir, 
            criticalChancePercent:criticalChancePercent, criticalDamagePercent:criticalDamagePercent);
            bulletScript.rb.velocity = dir * 7f;
        }
    }
}
