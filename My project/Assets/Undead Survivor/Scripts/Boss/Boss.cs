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
    public GameObject[] bossLevel;
    Collider2D coll;
    SpriteRenderer sprite;
    BossWeapon[] weapons;
    bool isPhase2;
    float Timer;
    int levelIndex;
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

    void Update() {
        if (!GameManager.Instance.isLive) return;
        if (!isLive) return;
        if (Input.GetKeyDown(KeyCode.H)) {
            isPhase2 = true;
        }
        if (isPhase2) {
            Timer += Time.deltaTime;
            if(Timer > 5f) {
                Timer = 0f;
                bossLevel[levelIndex++].SetActive(true);
                levelIndex = Mathf.Min(bossLevel.Length - 1, levelIndex);
            }
        }
        
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
        if (!isLive) return;

        health -= collision.GetComponent<Bullet>().damage;

        
        if(health < 0) {
            isLive = false;
            coll.enabled = false;
            sprite.sortingOrder = 1;
            anim.SetBool("Dead", true);
            
        }

    }
    void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Floor")) return;
        if (!isLive) return;

        health -= collision.GetComponent<FloorWeapon>().damage;

        if (health <= 0) {
            isLive = false;
            coll.enabled = false;
            sprite.sortingOrder = 1;
            anim.SetBool("Dead", true);

        }
    }
}
