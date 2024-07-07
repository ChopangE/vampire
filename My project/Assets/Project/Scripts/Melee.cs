using System.Collections;
using System.Collections.Generic;
//using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class Melee : MonoBehaviour
{
    private float curTime;
    public int id;
    public float damage = 3f;
    public float coolTime = 3f;
    public Vector2 boxSize;
    public Transform pos;
    Vector3 dir;

    [Header("# Object Info")]
    public GameObject sword; //��ǵ��� ������ ����. 
    public GameObject stick;
    GameObject sw;
    bool isGoing = false;
    SpriteRenderer sprite;
    void Start() {
        Player player = GetComponentInParent<Player>();
        //sw = Instantiate(sword, transform);
        //sw.transform.position = Vector3.zero;
        sprite = player.GetComponent<SpriteRenderer>();
        boxSize = stick.GetComponent<BoxCollider2D>().size;
        //dir = new Vector3(0, 0, 0);
        //dir.x = sprite.flipX ? 2f : -2f;
        //sw.transform.localPosition = dir;
        //sw.SetActive(false);
        //sw.tag = "Bullet";
        //Bullet bull = sw.AddComponent<Bullet>();
        //bull.damage = damage;
        //bull.per = -1;
       
    }

    void Update()
    {
        if (!GameManager.Instance.isLive) return;

        if (id == 1) {
            if (curTime <= 0) {
                if (Input.GetKeyDown(KeyCode.Z)) {
                    dir = new Vector3(0.3f, 0, 0);
                    if (pos.gameObject.GetComponent<SpriteRenderer>().flipX) {
                        dir = dir * -1;
                    }
                    GameObject sw = Instantiate(stick, pos.position + dir, Quaternion.identity);
                    sw.transform.parent = pos.transform;
                    sw.transform.localScale = sprite.flipX? new Vector3(-1,1,1) : new Vector3(1,1,1);
                    Destroy(sw, 0.5f);
                    //Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(sw.transform.position, boxSize, 0);

                    //foreach (Collider2D coll in collider2Ds) {
                    //    if (coll.CompareTag("Enemy") && coll.GetComponent<Enemy>() != null) {
                    //        coll.GetComponent<Enemy>().GetDamage(damage);
                    //    }
                    //    else if (coll.CompareTag("BossEnemy") && coll.GetComponent<BossEnemy>() != null) {
                    //        coll.GetComponent<BossEnemy>().GetDamage(damage);

                    //    }
                    //}
                    curTime = coolTime;
                }

            }
            curTime -= Time.deltaTime;
        } 
        else {
            if (Input.GetKeyDown(KeyCode.Z) && !isGoing) {
                isGoing = true;
                dir.x = sprite.flipX ? 2f : -2f;
                sw.transform.localPosition = dir;
                sw.SetActive(true);
            }
            if (isGoing) {
                curTime += Time.deltaTime;
                transform.Rotate(Vector3.back * 360 * Time.deltaTime);
            }
            if(curTime >= 1f) {
                transform.rotation = Quaternion.identity;
                isGoing = false;
                curTime = 0f;
                sw.SetActive(false);
            }
        }
    }
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos.position + dir, stick.GetComponent<BoxCollider2D>().size);
    }
    #region animation
    //public void ObjectDestroy() {
    //    Destroy
    //}
    #endregion
}
