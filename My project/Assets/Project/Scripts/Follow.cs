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
        rect.position =
            Camera.main.WorldToScreenPoint(player.transform
                .position); //GameManager.Instance.player.transform.position);
    }
}
