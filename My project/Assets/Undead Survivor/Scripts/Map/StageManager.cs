using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StageType {
    Starting, Stage, Shop, Random, Boss, Rest
}
public class StageManager : MonoBehaviour
{
    private static StageManager instance;
    public static StageManager Instance {
        get {
            if (instance == null) {
                var obj = FindObjectOfType<StageManager>();
                if (obj != null) {
                    instance = obj;
                }
                else {
                    var newObj = new GameObject().AddComponent<StageManager>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }


    public int curPoint = 0;
    public bool[] isCheck = { true, false, false, false, false, false };
    public int coin;
    Text coinText;
    void Awake() {
        var objs = FindObjectsOfType<StageManager>();
        if (objs.Length != 1) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    public void CallScene() {
        SceneManager.LoadScene(2);
        Debug.Log("Call");
    }
    
    public void TextingCoin() {
        coinText = FindObjectOfType<CoinText>().coinText;
        coinText.text = string.Format("Coin : {0}", coin);  
    }

}
