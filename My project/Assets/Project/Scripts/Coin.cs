using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Sprite[] sprites; 
    public int exp = 0;
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();    
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        GameManager.Instance.GetExp(exp);
        gameObject.SetActive(false);
    }
}
