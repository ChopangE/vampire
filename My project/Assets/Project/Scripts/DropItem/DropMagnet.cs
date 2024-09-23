using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;


namespace InGame
{
    public class DropMagnet : DropItem
    {
        private Collider2D coll;
        private Vector3 offset;
        private Player player;
        public float duration;
        protected override void Start()
        {
            base.Start();
            coll = GetComponent<Collider2D>();
            offset = new Vector3(0, 2f, 0);
            player = GameManager.Instance.player;
            dropItemTrigger.OnTriggered += PullCoin;
        }

        private void PullCoin()
        {
            coll.enabled = false;
            transform.position = player.transform.position + offset;
            transform.parent = player.transform;
            transform.DOShakePosition(duration, new Vector3(0, 0.5f, 0),1,0f, false, false);
            Coin[] coins = FindObjectsOfType<Coin>();
            DOVirtual.DelayedCall(duration,() => { DestroyItem(); }).OnUpdate(() => { 
                coins =  FindObjectsOfType<Coin>();
                foreach (var coin in coins)
                {
                    coin.transform.DOMove(player.transform.position, 0.3f);
                }
            }).OnComplete(() =>
            {
                coins = FindObjectsOfType<Coin>();
                foreach (var coin in coins)
                {
                    coin.transform.DOMove(player.transform.position, 0.3f);
                }
            });
            
        }
    }
}

