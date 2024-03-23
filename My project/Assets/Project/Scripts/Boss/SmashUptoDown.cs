using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashUptoDown : MonoBehaviour {
    Player player;
    SpriteRenderer sprite;
    void Awake() {
        player = GameManager.Instance.player;
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    void Start() {
        sprite.color = new Color(1, 1, 1, 0);
    }


    public void OnColor() {
        sprite.color = new Color(1, 1, 1, 1);

    }
    public void OffColor() {
        sprite.color = new Color(1, 1, 1, 0);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;
        GameManager.Instance.health -= 10f;
    }
    // Update is called once per frame

    /*
    timer += Time.deltaTime;
    if(timer > 3f) {
        timer = 0;
        sprite.color = new Color(1, 1, 1, 0);
    }
    */
    //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 2.5f * Time.deltaTime);


    //sprite.color = new Color(1, 1, 1, 1);
    //transform.position = player.transform.position;
}

