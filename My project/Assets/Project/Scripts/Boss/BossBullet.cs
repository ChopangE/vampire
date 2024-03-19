using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float damage;
    public float duration;
    SpriteRenderer sprite;
    Collider2D coll;
    BossWeaponType type;
    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
    }
    void Update() {
        if (!GameManager.Instance.isLive) return;
        if(type == BossWeaponType.Range) {
            transform.Translate(0, -3f * Time.deltaTime, 0);
        }
    }

    public void Init(float Damage, Vector3 dir, BossWeaponType bwt) {
        type = bwt;
        damage = Damage;
        switch (bwt) {
            case BossWeaponType.Bomb:
                StartCoroutine(PreStop());
                StartCoroutine(Stop());
                break;
            case BossWeaponType.Range:
                BossBullet[] childBullet = GetComponentsInChildren<BossBullet>();
                foreach (BossBullet bb in childBullet) {
                    bb.damage = damage;
                }
                StartCoroutine(Stop());

                break;
        }
    }
    IEnumerator PreStop() {
        sprite.color = new Color(0.8f, 0, 0, 0.5f);
        coll.enabled = false;
        yield return new WaitForSeconds(0.8f);
        sprite.color = Color.white;
        coll.enabled = true;
    }
    IEnumerator Stop() {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;
        GameManager.Instance.health -= damage;

    }
}
