using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Manager;

namespace InGame
{
    public class MagnetItem : ActiveItemBase
    {
        [SerializeField] private float pullSpeed = 15f;
        
        private Collider2D coll;
        private Vector3 offset;
        private Player player;
        
        protected override void Start()
        {
            base.Start();
            coll = GetComponent<Collider2D>();
            offset = new Vector3(0, 2f, 0);
            player = GameManager.Instance?.player;
        }
        
        public override void ActivateEffect()
        {
            Debug.Log($"자석 효과 시작 (지속 시간: {duration}초)");
            
            if (player == null)
            {
                player = GameManager.Instance?.player;
                if (player == null)
                {
                    Debug.LogWarning("플레이어 참조를 찾을 수 없습니다.");
                    EndEffect();
                    return;
                }
            }
            
            // DropMagnet과 동일한 시각적 효과 적용
            if (coll != null)
            {
                coll.enabled = false;
            }
            
            // 플레이어 위에 위치시키고 부모로 설정
            transform.position = player.transform.position + offset;
            transform.parent = player.transform;
            
            // 흔들림 효과 적용
            transform.DOShakePosition(duration, new Vector3(0, 0.3f, 0), 1, 0f, false, false);
            
            // 아이템 끌어당기기 시작
            StartCoroutine(PullItems());
        }
        
        private IEnumerator PullItems()
        {
            // 효과 지속 시간 동안 반복
            float endTime = Time.time + duration;
            
            while (Time.time < endTime)
            {
                if (player != null)
                {
                    // 주변 경험치 아이템 찾기
                    var expList = Global.ExpManager.spawnedItemList;
                    
                    // 각 아이템을 플레이어 방향으로 이동 (DropMagnet과 동일한 방식)
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
                
                yield return null;
            }
            
            Debug.Log("자석 효과 종료");
            EndEffect();
        }
        
        protected override void EndEffect()
        {
            // 모든 코루틴과 트윈 중지
            StopAllCoroutines();
            DOTween.Kill(transform);
            
            // 기본 종료 처리
            base.EndEffect();
        }
    }
}