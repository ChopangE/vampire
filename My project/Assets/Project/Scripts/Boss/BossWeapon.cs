using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Weapon;

public enum BossWeaponType {
    Bomb, Range
}
public class BossWeapon : MonoBehaviour
{
    
    public BossPoolManager poolManager;
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    public BossWeaponType WT;
    int bossWeaponPat;
    float timer;
    Player player;
    Boss boss;

    void Awake() {
        player = GameManager.Instance.player;
        boss = GetComponentInParent<Boss>();
    }

    void Update() {
        if (!GameManager.Instance.isLive) return;
        timer += Time.deltaTime;
        if (timer > 3f) {
            timer = 0f;
            bossWeaponPat = Random.Range(0, 2);
            string aniName = "Pattern" + bossWeaponPat;
            if (bossWeaponPat == 1) bossWeaponPat = 2;
            boss.anim.SetTrigger(aniName);

        }
    }
            /*
            switch (WT) {
                case BossWeaponType.Bomb:
                    if (timer > 5f) {
                        isPatterning = true;
                        boss.anim.SetTrigger("Pattern0");
                        timer = 0f;
                    }
                    break;
                case BossWeaponType.Range:
                    if (timer > 3f) {
                        isPatterning = true;
                        boss.anim.SetTrigger("Pattern1");
                        timer = 0f;
                    }
                    break;
                default:
                    break;
            }
            */
        
        /*
        if (BossManager.curTimer < 3f) {
            BossManager.curTimer += Time.deltaTime;
        }
        else {
            BossManager.curTimer = 0f;
            BossManager.isPatterning = false;
        }
        */
        //Invoke("isPatteringFalse", 3f);


    public void Shut(int prefabId, int damage) {
        Transform bullet = poolManager.Get(prefabId).transform;
        bullet.parent = transform;
        bullet.position = player.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        bullet.localRotation = Quaternion.identity;
        Transform bottle = poolManager.Get(prefabId + 1).transform;
        bottle.parent = transform;
        bottle.position = bullet.position + new Vector3(0.4f,3.5f,0);
        bottle.GetComponent<Bottle>().target = bullet.position + new Vector3(0.4f,0.4f,0);
        bullet.GetComponent<BossBullet>().Init(damage, Vector3.zero, BossWeaponType.Bomb);
    }

    public void Range(int prefabId, int damage) {
        Transform bullet = poolManager.Get(prefabId).transform;
        bullet.parent = transform;
        bullet.position = new Vector3(player.transform.position.x, boss.transform.position.y - 2 , 0);
        bullet.GetComponent<BossBullet>().Init(damage, Vector3.zero, BossWeaponType.Range);


    }
}
