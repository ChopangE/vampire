using System;
using System.Collections;
using System.Collections.Generic;
using InGame;
using UnityEngine;

public class DefaultMagnet : MonoBehaviour
{
    private Player player;
    public float pullSpeed;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.GetComponent<ExpItem>()) return;
        Vector3 pos = Vector3.MoveTowards(other.transform.position, player.transform.position,
            Time.deltaTime * pullSpeed);
        other.transform.position = pos;
    }
}
