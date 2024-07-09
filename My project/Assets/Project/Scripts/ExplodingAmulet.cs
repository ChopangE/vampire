using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ExplodingAmulet : MonoBehaviour
{
    Player player;
    public float speed;
    Vector3 dir;
    SpriteRenderer sprite;

    public Animator childAnim;
    void Awake() {
        player = GameManager.Instance.player;
        sprite = GetComponent<SpriteRenderer>();
    }
    void OnEnable() {
        //transform.position = player.transform.position;
        //transform.position = new Vector3(0, 0, 0);
        sprite.color = new Color(1, 1, 1, 1);
        transform.GetChild(0).gameObject.SetActive(false);
        dir = player.GetComponent<SpriteRenderer>().flipX ? new Vector3(-1 * speed, 0, 0) : new Vector3(1 * speed, 0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(dir * Time.deltaTime);
        if(transform.GetChild(0).gameObject.activeSelf && childAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {  //애니메이션 실행중인 것을 확인하는 코드 삽입해야됨.
            gameObject.SetActive(false);
        }
    }

    public void Exploding() {
        sprite.color = new Color(1,1,1,0);
        transform.GetChild(0).gameObject.SetActive(true);
        dir = Vector3.zero;
    }
}
