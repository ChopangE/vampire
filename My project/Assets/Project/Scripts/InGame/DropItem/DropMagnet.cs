using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
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
        public float pullSpeed;
        protected override void Awake()
        {
            base.Awake();
            coll = GetComponent<Collider2D>();
            offset = new Vector3(0, 2f, 0);
            player = GameManager.Instance.player;
        }

        private void OnEnable()
        {
            dropItemTrigger.OnTriggered += PullCoin;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            dropItemTrigger.OnTriggered -= PullCoin;
        }
        private void PullCoin()
        {
            coll.enabled = false;
            transform.position = player.transform.position + offset;
            transform.parent = player.transform;
            transform.DOShakePosition(duration, new Vector3(0, 0.3f, 0),1,0f, false, false);
            var expList = Global.ExpManager.spawnedItemList;
            player = GameManager.Instance.player;
            DOVirtual.DelayedCall(duration, () => { DestroyItem(); }).OnUpdate(() =>
            {
                //List<Tween> tweens = new List<Tween>();
                foreach (var expItem in expList)
                {
                    Vector3 pos = Vector3.MoveTowards(expItem.transform.position, player.transform.position,
                        Time.deltaTime * pullSpeed);
                    expItem.transform.position = pos;
                    //expItem.transform.DOMove(player.transform.position, 0.5f);
                }
                // foreach (var tween in tweens)
                // {
                //     tween.Kill(false);
                // }
            });

        }
    }
}

