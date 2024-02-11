using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float gameTime;
    public float maxGameTime = 2 * 10f;

    public Player player;
    public PoolManager pool;
    void Awake() {
        Instance = this;
    }

    void Update() {
        gameTime += Time.deltaTime;
        if(gameTime > maxGameTime) {
            gameTime = maxGameTime;
        }
    }

}
