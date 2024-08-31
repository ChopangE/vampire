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
        public int maxStageNum = 3;
        public int maxStageCountNum = 4;
        public int stageLevel = 0;
        public int stageCount = 0;

        public int curPoint = 0;
        public bool[] isCheck = { true, false, false, false, false, false };
        public int coin;
        Text coinText;

        public void TextingCoin()
        {
            coinText = FindObjectOfType<CoinText>().coinText;
            coinText.text = string.Format("Coin : {0}", coin);
        }

    }
}