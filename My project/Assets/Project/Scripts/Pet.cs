using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    //public Transform playerPos;
    private Transform playerPos;
    public SpriteRenderer player;
    public float speed;
    float timer;
    SpriteRenderer sprite;
    void Awake()
    {
        playerPos = GameManager.Instance.player.transform;
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        transform.position = playerPos.position;

    }
    void Update()
    {
        if (!GameManager.Instance.isLive) return;
        if((transform.position - playerPos.position).magnitude > 10)
        {
            transform.position = playerPos.position;
        }
        Vector3 dir = new Vector3(0, 1.5f, 0);
        dir.x = player.flipX ? 1.5f : -1.5f;
        sprite.flipX = !player.flipX;
        transform.position = Vector3.MoveTowards(transform.position, playerPos.position + dir , 3f * Time.deltaTime);
        
    }

    
}
