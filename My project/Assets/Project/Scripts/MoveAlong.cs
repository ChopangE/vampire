using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlong : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GameManager.Instance.player;
    }

    

    // Update is called once per frame
    void Update()
    {
       transform.position = player.transform.position;
    }
}
