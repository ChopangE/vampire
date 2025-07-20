using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace InGame
{
    public class DropMagnet : DropItem
    {
        private Collider2D coll;
        private Vector3 offset;
        private Player player;
        public float duration;
        public float pullSpeed;
        private bool isEffectActive = false;
        
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
            transform.DOShakePosition(duration, new Vector3(0, 0.3f, 0), 1, 0f, false, false);
            
            player = GameManager.Instance.player;
            isEffectActive = true;
            
            // 자석 효과 시작
            StartMagnetEffect().Forget();
        }
        
        private async UniTaskVoid StartMagnetEffect()
        {
            var expList = Global.ExpManager.spawnedItemList;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration && isEffectActive)
            {
                // 게임이 일시정지되면 시간 카운트와 아이템 이동 모두 멈춤
                if (GameManager.Instance != null && GameManager.Instance.isLive)
                {
                    elapsedTime += Time.deltaTime;
                    
                    // 경험치 아이템들을 플레이어 방향으로 이동
                    foreach (var expItem in expList)
                    {
                        if (expItem != null)
                        {
                            Vector3 pos = Vector3.MoveTowards(expItem.transform.position, 
                                player.transform.position, Time.deltaTime * pullSpeed);
                            expItem.transform.position = pos;
                        }
                    }
                }
                
                await UniTask.Yield();
            }
            
            // 효과 종료
            if (isEffectActive)
            {
                DestroyItem();
            }
        }
        
        protected override void DestroyItem()
        {
            isEffectActive = false;
            DOTween.Kill(transform);
            base.DestroyItem();
        }
    }
}

