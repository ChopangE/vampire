using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StageType
{
    Starting, Stage, Shop, Boss, Random, Rest
}
namespace Manager
{
    public class StageManager : MonoBehaviour
    {
        public event Action OnStageChanged;
        public int stageCount { get { return Global.UserDataManager.curStage % MAX_STAGE_COUNT; } }
        public int stageLevel { get { return Global.UserDataManager.curStage / MAX_STAGE_COUNT; } } 

        public readonly int MAX_STAGE_COUNT = 4;
        public readonly int MAX_STAGE_LEVEL = 3;

        public void ChangeStage(int stage)
        {
            Global.UserDataManager.curStage = stage;
            OnStageChanged?.Invoke();
            Global.UserDataManager.Save();
        }
    }
}