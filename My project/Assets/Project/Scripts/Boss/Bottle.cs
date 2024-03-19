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
}
