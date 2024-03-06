using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CallLoading : MonoBehaviour
{
    public void OnClickLoadScene() {
        SceneManager.LoadScene(3);
    }

}
