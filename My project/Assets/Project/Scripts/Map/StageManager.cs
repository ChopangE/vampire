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
        public int stageCount { get { return Global.UserDataManager.curStage % 4; } }
        public int stageLevel { get { return Global.UserDataManager.curStage / 4; } } 

        public int MAX_STAGE_COUNT
        {
            get { return 4;}
            private set { }
        }
        public int MAX_STAGE_LEVEL
        {
            get { return 3;}
            private set { }
        }
       

    }
}