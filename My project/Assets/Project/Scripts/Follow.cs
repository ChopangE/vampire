using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    RectTransform rect;
    private Player player;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        player = FindObjectOfType<Player>();
    }


    void FixedUpdate()
    {
        if (player == null || Camera.main == null)
        {
            // 플레이어나 카메라가 없으면 다시 찾기 시도
            if (player == null)
                player = FindObjectOfType<Player>();
            
            if (player == null || Camera.main == null)
                return;
        }
        
        rect.position =
            Camera.main.WorldToScreenPoint(player.transform
                .position); //GameManager.Instance.player.transform.position);
    }
}
