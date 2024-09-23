using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace InGame
{
   public class DropCoin : DropItem
   {
       public int coin;
       // Start is called before the first frame update
       protected override void Start()
       {
           base.Start();
           dropItemTrigger.OnTriggered += PlusCoin;
       }

       void PlusCoin()
       {
           int nowCoin = int.Parse(Global.UserDataManager.GetGoldDataString());
           nowCoin += coin;
           Global.UserDataManager.SetGoldData(nowCoin.ToString());
           DestroyItem();
       }
   } 
}

