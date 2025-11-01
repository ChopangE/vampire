using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class IceFall : MiddleBossBullet
{

    Vector3 playerPos;
    SpriteRenderer sprite;
    CircleCollider2D coll;

    protected override void Init()
    {
        base.Init();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<CircleCollider2D>();
        coll.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(OnThunder());
    }



    IEnumerator OnThunder()
    {
        float timer = 0f;
        float duration = 2f;
        float startColor = 0.0f;
        float endColor = 1f;
        while (timer < duration)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            playerPos = player.transform.position + new Vector3(0f, -1f, 0f);
            transform.position = playerPos;
            timer += Time.deltaTime;
            sprite.color = new Color(1, 0, 0, Mathf.Lerp(startColor, endColor, timer / duration));
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);  //시간 조절 가능
        Attack();
    }

    void Attack()
    {
        Global.SoundManager.PlaySFX(SFXEnum.FrozenMelt);
        sprite.color = new Color(1, 0, 0, 0);
        coll.enabled = true;
        Transform ch = transform.GetChild(0);
        transform.localScale = new Vector3(1f, 1f, 1f);
        ch.gameObject.SetActive(true);
    }

    protected override void Damaging()
    {
        var damage = GameManager.Instance.maxHealth * 0.3f;
        GameManager.Instance.Health -= damage;
        // 2초 동안 30% 슬로우 적용
        GameManager.Instance.player.ApplySlow(2f, 0.3f);
    }
}
