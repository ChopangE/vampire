using Manager;
using UI.Page;
using UnityEngine;

namespace OutGame
{
    public class StartScene : SceneBase
    {
        public static float Speed = 1;
        // public TextMeshProUGUI flashingText;
        
        private OptionPage _optionPage;
        protected override void Start()
        {
            base.Start();
            Global.UIManager.OpenPage<IntroScenePage>();
            // SplashScreenTask().Forget();
        }
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape) && _optionPage == null)
                _optionPage = Global.UIManager.OpenPage<OptionPage>();
            else if (Input.GetKeyDown(KeyCode.Escape) && _optionPage != null)
                Global.UIManager.ClosePage();
        }
    }
}