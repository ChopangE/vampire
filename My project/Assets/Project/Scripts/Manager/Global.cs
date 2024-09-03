using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// 전역 접근을 위한 최상위 객체입니다.
    /// </summary>
    /// <remarks>
    /// Global.UIManager와 같은 방식으로 매니저 객체에 접근할 수 있습니다.
    /// </remarks>
    public class Global : MMSingleton<Global>
    {
        public static OutGame.SceneBase CurrentScene { get; set; }

        public static UIManager UIManager { get; private set; }
        public static UserDataManager UserDataManager { get; set; }
        public static SoundManager SoundManager { get; set; }
        public static StageManager StageManager { get; set; }
        public static StatsManager StatsManager { get; set; }
        public static StatsUpgradeManager StatsUpgradeManager { get; set; }

        // public static OptionManager OptionManager { get; private set; }
        // public static GameDataManager GameDataManager { get; set; }
        // public static SceneBase CurrentScene { get; private set; }
        // public static LocalizationManager LocalizationManager { get; private set; }
        // public static NewDialogueManager DialogueManager { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = false;

            LoadManagerPrefabs();
            UserDataManager = new();
            UserDataManager.Load();

        }

        private void LoadManagerPrefabs()
        {
            string prefixManager = "Prefabs/Manager/";
            if (UIManager == null)
            {
                UIManager = Instantiate(Resources.Load<UIManager>(prefixManager + nameof(Manager.UIManager)), transform);
                UIManager.name = nameof(UIManager);
            }
            if (SoundManager == null)
            {
                SoundManager = Instantiate(Resources.Load<SoundManager>(prefixManager + nameof(Manager.SoundManager)), transform);
                SoundManager.name = nameof(SoundManager);
            }
            if (StageManager == null)
            {
                StageManager = Instantiate(Resources.Load<StageManager>(prefixManager + nameof(Manager.StageManager)), transform);
                StageManager.name = nameof(StageManager);
            }
            if (StatsManager == null)
            {
                StatsManager = Instantiate(Resources.Load<StatsManager>(prefixManager + nameof(Manager.StatsManager)), transform);
                StatsManager.name = nameof(StageManager);
            }
            if (StatsUpgradeManager == null)
            {
                StatsUpgradeManager = Instantiate(Resources.Load<StatsUpgradeManager>(prefixManager + nameof(Manager.StatsUpgradeManager)), transform);
                StatsUpgradeManager.name = nameof(StageManager);
            }
        }
    }
}
