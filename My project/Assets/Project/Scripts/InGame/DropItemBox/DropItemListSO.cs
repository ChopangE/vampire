using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InGame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "DropItemList", menuName = "드랍아이템 리스트")]
    public class DropItemListSO : SerializedScriptableObject
    {
        [LabelText("드랍 가능한 아이템 리스트")]
        public List<GameObject> dropItemList;
        [Button("그냥 드랍아이템 전부 가져오기")]
        public void LoadAllDropItem()
        {
            dropItemList = Resources.LoadAll<GameObject>("Prefabs/DropItem").ToList();
        }
    }
}