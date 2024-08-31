using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI.Page;

namespace OutGame
{
    public class MapScene : SceneBase
    {
        protected override void Start()
        {
            base.Start();
            Global.UIManager.OpenPage<MapPage>();
        }
    }
}
