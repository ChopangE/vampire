using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class CoinText : MonoBehaviour
{
    public Text coinText;
    void Awake() {
        coinText = GetComponent<Text>();
    }
    void Start()
    {
        coinText.text = string.Format("Coin : {0}", Global.StageManager.coin);
    }

    
}
