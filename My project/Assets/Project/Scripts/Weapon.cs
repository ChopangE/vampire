using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    public enum weaponType {
        rotate, Range, Bomb, Raser,Breath
    }
    public weaponType WT;
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    float timer;
    Player player;
    public ItemData data; 
    void Awake() {
        player = GameManager.Instance.player;
    }
    
    void Update()
    {
        if (!GameManager.Instance.isLive) return;

        timer += Time.deltaTime;
        switch (id) {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            case 1:
                if (timer > speed) {
                    timer = 0f;
                    Fire();
                }
                break;
            case 2:
                if (timer > speed)
                {
                    timer = 0f;
                    Shut();
                }
                break;
            case 3:                 
                if (timer > speed) {
                    timer = 0f;
                    Amulet();
                }
                break;
            case 4:
                if(timer > speed) {
                    timer = 0f;
                    Breath();
                }
                break;
            default:
                break;
        }
        
    }

    //private void OnEnable()
    //{
    //    Init(data);
    //}

    public void LevelUp(float damage, int count) {
        this.damage = damage;
        this.count += count;
        
        if(id == 0) {
            Batch();
        }

        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);

    }
    public void Init(ItemData data) {

        // Basic Set
        //name = "Weapon " + data.itemId;
        //transform.parent = player.transform;
        //transform.localPosition = Vector3.zero;


        // Property Set
        id = data.itemId;
        damage = data.baseDamage;   
        count = data.baseCount;
        
        for(int i =0; i < GameManager.Instance.pool.prefabs.Length; i++) {
            if(data.projectile == GameManager.Instance.pool.prefabs[i]) {
                prefabId = i;
                break;
            }
        }
        InitSetting();
    }

    public void InitSetting() {
        switch (id) {
            case 0:                     //Shop
                speed = 150;
                Batch();
                break;
            case 1:                     //Fire
                speed = 0.3f;
                break;
            case 2:                     //Bomb
                speed = 3f;
                break;
            case 3:                     //Raser
                speed = 5f;
                break;
            case 4:                     //Breath
                speed = 5f;
                break;
            default:
                break;
        }

        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);
    }

    void Batch() {
        for(int index= 0; index < count; index++) {
            Transform bullet;
            if(index < transform.childCount) {
                bullet = transform.GetChild(index);
            }
            else {
                bullet = GameManager.Instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;
            
            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(Vector3.up * 1.5f, Space.Self);

            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero); // -1 is infinity per;
            
        }
    }

    void Fire() {
        if (!player.scan.nearestTarget) {
            return;
        }

        Vector3 targetPos = player.scan.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }

    void Shut()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        //bullet.parent = transform;
        bullet.position = transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        bullet.rotation = Quaternion.identity;
        bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero);
        
    }

    void Raser() {
        //Collider2D[] colliders = Physics2D.OverlapBoxAll(player.transform.position, new Vector2(10f, 3f), 50);
        Vector3 dir = player.inputVec;
        if(dir.magnitude < 0.1f) {
            dir = player.GetComponent<SpriteRenderer>().flipX ? new Vector2(-1f, 0) : new Vector2(1f, 0);
        }
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + dir * 5f;

        //bullet.position = transform.position + dir * 8f;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.Rotate(new Vector3(0, 0, -90), Space.Self);
        bullet.GetComponent<Bullet>().Init(damage, -1, dir);

        StartCoroutine(raser(bullet, dir));
    }
    void Amulet() {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.GetChild(0).GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
    }
    void Breath() {
        Vector3 dir = player.inputVec;
        if (dir.magnitude < 0.1f) {
            dir = player.GetComponent<SpriteRenderer>().flipX ? new Vector2(-1f, 0) : new Vector2(1f, 0);
        }
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + dir * 3f;
        //Vector3 dir2 = dir;
        //if (player.scan.nearestTarget) {
        //    Vector3 targetPos = player.scan.nearestTarget.position;
        //    dir2 = (targetPos - transform.position).normalized;
        //}
        //bullet.transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector3.up, dir2);
        //bullet.transform.GetChild(0).GetComponent<Bullet>().Init(damage, 1, dir2);
        //StartCoroutine(BreathDown(bullet, dir));
    }
    IEnumerator BreathDown(Transform bullet, Vector3 dir) {
        yield return new WaitForSeconds(0.4f);
        bullet.localScale = new Vector3(5, 5, 0);
        bullet.position = bullet.position + dir * 4f;
        yield return new WaitForSeconds(0.5f);
        bullet.gameObject.SetActive(false);
    }
    IEnumerator raser(Transform bullet, Vector3 dir) {
        Vector3 sc = bullet.localScale;
        yield return new WaitForSeconds(0.7f);
        bullet.localScale = new Vector3(5f, 3f, 1);
        bullet.position = transform.position + dir * 8f;
        yield return new WaitForSeconds(0.7f);
        bullet.gameObject.SetActive(false);
        bullet.localScale = sc;
    }
}
