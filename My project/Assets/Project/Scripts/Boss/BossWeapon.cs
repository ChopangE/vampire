using System.Collections;
using System.Collections.Generic;
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
        switch (WT) {
            case BossWeaponType.Bomb:
                if (timer > 5f) {
                    boss.anim.SetTrigger("Pattern0");             
                    timer = 0f;
                }
                break;
            case BossWeaponType.Range:
                if (Input.GetKeyDown(KeyCode.K)) {
                    boss.anim.SetTrigger("Pattern1");

                }
                break;
            default:
                break;
        }
    }


    public void Shut() {
        Transform bullet = poolManager.Get(prefabId).transform;
        bullet.parent = transform;
        bullet.position = player.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        bullet.localRotation = Quaternion.identity;
        Transform bottle = poolManager.Get(prefabId + 1).transform;
        bottle.parent = transform;
        bottle.position = bullet.position + new Vector3(0.4f,3.5f,0);
        bottle.GetComponent<Bottle>().target = bullet.position + new Vector3(0.4f,0.4f,0);
        bullet.GetComponent<BossBullet>().Init(damage, Vector3.zero,WT);

    }

    public void Range() {
        Transform bullet = poolManager.Get(prefabId).transform;
        bullet.parent = transform;
        //bullet.position = new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, 0);
        //transform.position + new Vector3(Random.Range(-15f, 5f),-3f, 0);
        bullet.position = new Vector3(player.transform.position.x, boss.transform.position.y - 2 , 0);
        //bullet.localPosition = new Vector3(Random.Range(-3f,3f), 0, 0);
        
        bullet.GetComponent<BossBullet>().Init(damage, Vector3.zero, WT);


    }
}
