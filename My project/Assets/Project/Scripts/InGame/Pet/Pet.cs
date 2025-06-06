using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    //public Transform playerPos;
    private Transform playerPos;
    public SpriteRenderer player;
    public float speed = 2f;
    public float followDistance = 0.2f; // 펫이 따라가기 시작하는 최소 거리
    float timer;
    SpriteRenderer sprite;
    
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if(GameManager.Instance != null && GameManager.Instance.player != null)
        {
            playerPos = GameManager.Instance.player.transform;
            transform.position = playerPos.position;
        }
    }
    
    void FixedUpdate()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isLive) return;
        
        if(playerPos == null && GameManager.Instance.player != null)
        {
            playerPos = GameManager.Instance.player.transform;
        }
        
        if(playerPos == null) return;
        
        // 플레이어와 너무 멀리 떨어진 경우 순간이동
        if((transform.position - playerPos.position).magnitude > 10)
        {
            transform.position = playerPos.position;
        }
        
        Vector3 dir = new Vector3(0f, 1.2f, 0);
        dir.x = player.flipX ? 1.2f : -1.2f;
        sprite.flipX = !player.flipX;
        
        Vector3 targetPosition = playerPos.position + dir;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        
        // 목표 위치와의 거리가 followDistance보다 클 때만 이동
        if (distanceToTarget > followDistance)
        {
            // Lerp를 사용해 부드러운 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }
}
