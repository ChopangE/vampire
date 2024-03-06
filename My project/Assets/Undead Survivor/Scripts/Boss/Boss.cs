using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public Rigidbody2D target;
    bool isLive;
    public Animator anim;
    Collider2D coll;
    SpriteRenderer sprite;
    BossWeapon[] weapons;
    void Awake() {
        coll = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        weapons = GetComponentsInChildren<BossWeapon>();
    }
    public void Bottle() {
        weapons[0].Shut();
    }
    public void Crows() {
        weapons[1].Range();
    }
    
    void OnEnable() {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        sprite.sortingOrder = 1;
        health = maxHealth;
    }
    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Bullet")) return;

        health -= collision.GetComponent<Bullet>().damage;

        
        if(health < 0) {
            isLive = false;
            coll.enabled = false;
            sprite.sortingOrder = 1;
            anim.SetTrigger("Dead");
        }

    }
    void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Floor")) return;
        health -= collision.GetComponent<FloorWeapon>().damage;

        if (health <= 0) {
            isLive = false;
            coll.enabled = false;
            sprite.sortingOrder = 1;
        }
    }
}
