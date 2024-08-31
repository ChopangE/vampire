using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Manager;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UI.Page;

namespace OutGame
{
    public class StartScene : SceneBase
    {
        public static float Speed = 1;
        // public TextMeshProUGUI flashingText;
        
        protected override void Start()
        {
            base.Start();
            Global.UIManager.OpenPage<StartScenePage>();
            // SplashScreenTask().Forget();
        }
    }
}