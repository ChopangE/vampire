using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetWeapon : MonoBehaviour
{
    public int prefabId;
    public float damage;
    public float speed;
    float timer;
    public Player player;

    void Start() {
        speed = 1f;
    }
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isLive) return;

        timer += Time.deltaTime;
        if (timer > speed)
        {
            timer = 0f;
            Fire();
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
        bullet.GetComponent<Bullet>().Init(damage, 100, dir);
    }
}
