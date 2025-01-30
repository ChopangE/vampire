using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace InGame
{
   public class DropLevelUpScroll : DropItem
   {
       protected override void Awake()
       {
           base.Awake();
       }

       void OnEnable()
       {
           dropItemTrigger.OnTriggered += ShowLevelUpScroll;
           
       }

       protected override void OnDisable()
       {
           base.OnDisable();
           dropItemTrigger.OnTriggered -= ShowLevelUpScroll;
       }
       void ShowLevelUpScroll()
       {
            GameManager.Instance.LevelUp(true);  
            DestroyItem();
       }
   } 
}

