using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    public Vector3 target;

    
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, 5 * Time.deltaTime);
        if(transform.position == target) {
            gameObject.SetActive(false);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        GameManager.Instance.Health -= GameManager.Instance.Health * 0.1f;

    }
}
