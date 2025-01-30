using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using SO;
using Manager.InGame;

public class ShadowPlayer : Melee
{
    private GameObject shadowPlayer;

    private SpriteRenderer curPlayerSpriteRenderer;
    private SpriteRenderer shadowPlayerSpriteRenderer;
    protected override void Start()
    {
        InitializeComponents();
        curPlayerSpriteRenderer = GameManager.Instance.player.GetComponent<SpriteRenderer>();
    }

    public override void LevelUp(UpgradeName prevUpgradeName, float prevUpgradeValue, DamageUpgradeValues damageUpgradeValues = null)
    {
        base.LevelUp(prevUpgradeName, prevUpgradeValue, damageUpgradeValues);
        GameManager.Instance.weaponController.bonusDamage = 0.2f + ((level - 1) * 0.3f);

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
            GameManager.Instance.weaponController.isBonusDamage = true;
            shadowPlayerSpriteRenderer = shadowPlayer.GetComponent<SpriteRenderer>();
            shadowPlayerSpriteRenderer.sprite = curPlayerSpriteRenderer.sprite;
            shadowPlayer.transform.localScale = Vector3.one;
        }
        else
        {
            GameManager.Instance.weaponController.isBonusDamage = false;
        }

    }
    public override async UniTask Init()
    {
        await base.Init();
    }

    public override void ExecuteAttack()
    {
        SetAttackDirection();
        shadowPlayer = SpawnSword();
        ConfigureSword(shadowPlayer, transform, true, new Vector3(-0.405f, 0, 0));
    }


}