using System;
using System.Collections;
using System.Collections.Generic;
using InGame;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DefaultMagnet : MonoBehaviour
{
    [SerializeField] private Collider2D coll;
    private Player player;
    public float pullSpeed;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
        coll = GetComponent<Collider2D>();
        UpdateMagnetRange();
    }

    private void UpdateMagnetRange()
    {
        if (coll != null)
        {
            // CircleCollider2D인 경우를 가정
            if (coll is CircleCollider2D circleCollider)
            {
                circleCollider.radius *= (1f + GameManager.Instance.expRangeBonus);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.GetComponent<ExpItem>()) return;
        Vector3 pos = Vector3.MoveTowards(other.transform.position, player.transform.position,
            Time.deltaTime * pullSpeed);
        other.transform.position = pos;
    }
}
