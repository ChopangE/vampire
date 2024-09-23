using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;


namespace InGame
{
    public class DropSpeedUp : DropItem
    {   
        
        [LabelText("5%면 0.05를 기입")]
        public float SpeedUpRatio;

        private Player player;
        private Collider2D coll;
        private Vector3 offset;
        public float duration;
        protected override void Start()
        {
            base.Start();
            coll = GetComponent<Collider2D>();
            player = GameManager.Instance.player;
            offset = new Vector3(0, 2f, 0);
            dropItemTrigger.OnTriggered += SpeedUp;
        }
        

        void SpeedUp()
        {
            player.speed *= (1f + SpeedUpRatio);
            coll.enabled = false;
            transform.position = player.transform.position + offset;
            transform.parent = player.transform;
            transform.DOShakePosition(duration, new Vector3(0, 0.5f, 0),1,0f, false, false);
            DOVirtual.DelayedCall(duration, SpeedDown);
        }

        void SpeedDown()
        {
            player.speed /= (1f + SpeedUpRatio);
            DestroyItem();
        }
    }

}
