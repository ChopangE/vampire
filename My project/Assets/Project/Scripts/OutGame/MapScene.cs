using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UI.Page;

namespace OutGame
{
    public class MapScene : SceneBase
    {
        protected override void Start()
        {
            base.Start();
            Global.UIManager.OpenPage<MapPageNew>();
        }
    }
}
