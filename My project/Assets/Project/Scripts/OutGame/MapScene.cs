using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UI.Page;
using UnityEngine;

namespace OutGame
{
    public class MapScene : SceneBase
    {
        private OptionPage _optionPage;
        protected override void Start()
        {
            base.Start();
            Global.UIManager.OpenPage<MapPageNew>();
        }
        
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape) && _optionPage == null)
                _optionPage = Global.UIManager.OpenPage<OptionPage>();
            else if (Input.GetKeyDown(KeyCode.Escape) && _optionPage != null)
                Global.UIManager.ClosePage();
        }
    }
}
