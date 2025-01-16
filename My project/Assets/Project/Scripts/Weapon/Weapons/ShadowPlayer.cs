using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class ShadowPlayer : Melee
{
    private GameObject shadowPlayer;

    private Sprite curPlayerSprite;
    protected override void Start()
    {
        InitializeComponents();
    }
    public override void Attack()
    {
        base.Attack();
        if(playerSprite == null)
            return;
        if(playerSprite.flipX)
            transform.localScale = new Vector3(-1, 1, 1);
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if(shadowPlayer != null)
        {
            GameManager.Instance.player.isBonusDamage = true;
            curPlayerSprite = GameManager.Instance.player.GetComponent<SpriteRenderer>().sprite;
            shadowPlayer.GetComponent<SpriteRenderer>().sprite = curPlayerSprite;
            shadowPlayer.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            GameManager.Instance.player.isBonusDamage = false;
        }

    }
    public override async UniTaskVoid Init(ItemData data)
    {
        base.Init(data).Forget();
        await UniTask.Yield();
        maxCooldown = 1f;
        remainingCooldown = maxCooldown;
    }

    public override void ExecuteAttack()
    {
        SetAttackDirection();
        shadowPlayer = SpawnSword();
        ConfigureSword(shadowPlayer, transform, true, new Vector3(-0.405f, 0, 0));
    }


}