using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildAnim : MonoBehaviour
{
    Transform parent;
    void Start() {
        parent = transform.parent;    
    }
    public void Stop() {
        parent.gameObject.SetActive(false); 
    }
}
