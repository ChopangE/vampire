using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashUptoDown : MonoBehaviour
{
    Player player;
    SpriteRenderer sprite;
    float timer;
    void Awake() {
        player = GameManager.Instance.player;
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*
        timer += Time.deltaTime;
        if(timer > 3f) {
            timer = 0;
            sprite.color = new Color(1, 1, 1, 0);
        }
        */
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 2.5f * Time.deltaTime);
    }

    void OnEnable() {
        sprite.color = new Color(1, 1, 1, 1);
        transform.position = player.transform.position;
    }
}
