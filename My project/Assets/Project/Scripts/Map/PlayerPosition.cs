using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPosition : MonoBehaviour
{
    public Transform[] Position;
    void Start() {
        transform.position = Position[StageManager.Instance.curPoint].position;
    }
}
