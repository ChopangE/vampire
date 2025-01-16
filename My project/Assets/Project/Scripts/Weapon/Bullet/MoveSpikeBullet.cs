using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpikeBullet : Bullet
{
    [SerializeField] private float moveSpeed = 2f;
    private Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();
        playerTransform = GameManager.Instance.player.transform;
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.isLive) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
