using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Thunder : MiddleBossBullet {

    Vector3 playerPos;
    SpriteRenderer sprite;
    CircleCollider2D coll;
    
    protected override void Init() {
        base.Init();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<CircleCollider2D>();
        coll.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(OnThunder());
    }


   
    IEnumerator OnThunder() {
        float timer = 0f;
        float duration = 2f;
        float startColor = 0.0f;
        float endColor = 1f;
        while(timer < duration) {
            playerPos = player.transform.position + new Vector3(0f, -1f, 0f);
            transform.position = playerPos;
            timer += Time.deltaTime;
            sprite.color = new Color(1, 0, 0, Mathf.Lerp(startColor, endColor, timer / duration));
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);  //시간 조절 가능
        Attack();
    }

    void Attack() {
        sprite.color = new Color(1, 0, 0, 0);
        coll.enabled = true;
        Transform ch = transform.GetChild(0);
        ch.gameObject.SetActive(true);
        
    }
}
