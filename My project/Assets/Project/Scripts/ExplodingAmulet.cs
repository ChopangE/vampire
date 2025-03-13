using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ExplodingAmulet : MonoBehaviour
{
    Player player;
    public float speed;
    Vector3 dir;
    SpriteRenderer sprite;
    public Animator childAnim;
    public GameObject[] Explosions;
    
    void Awake() {
        player = GameManager.Instance.player;
        sprite = GetComponent<SpriteRenderer>();
    }
    void OnEnable() {
        sprite.color = new Color(1, 1, 1, 1);
        transform.GetChild(0).gameObject.SetActive(false);
        
        // 랜덤한 각도 생성 (0~360도)
        float randomAngle = Random.Range(0f, 360f);
        // 각도를 라디안으로 변환하여 방향 벡터 계산
        dir = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * speed,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * speed,
            0
        );
        
        foreach(var explosion in Explosions) {
            explosion.SetActive(false);
        }
    }
    public virtual void Update()
    {
        transform.Translate(dir * Time.deltaTime);
        if(transform.GetChild(0).gameObject.activeSelf && childAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {  //애니메이션 실행중인 것을 확인하는 코드 삽입해야됨.
            gameObject.SetActive(false);
        }   
    }

    public void Exploding() {
        Global.SoundManager.PlaySFX(Data.SFXEnum.ExplosionCharm);
        sprite.color = new Color(1,1,1,0);
        transform.GetChild(0).gameObject.SetActive(true);
        dir = Vector3.zero;
        foreach(var explosion in Explosions) {
            explosion.SetActive(true);
        }
    }
}
