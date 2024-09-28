using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class StageDropItemBox : DropItemBox
    {
        protected override void OnTriggered()
        {
            base.OnTriggered();
            SpawnRandomDropItem();
        }

        //* 랜덤 드롭 아이템 소환
        private void SpawnRandomDropItem()
        {
            var index = UnityEngine.Random.Range(0, dropItemListSO.dropItemList.Count);
            SpawnDropItem(index);
        }

        private void SpawnDropItem(int index)
        {
            dropItemPoolManager.SpawnDropItem(index, transform.position);
        }
    }
}