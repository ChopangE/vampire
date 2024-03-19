using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class BackGround : MonoBehaviour
{


    void Update() {
        transform.Translate(new Vector3(5 * Time.deltaTime, 0, 0));
        if(transform.position.x > 119.25) {
            transform.Translate(new Vector3(-40 , 0, 0));
        }
    }
}
