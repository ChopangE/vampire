using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Video : MonoBehaviour
{
    
    VideoPlayer player;
    public RawImage rw;
    public bool isPlaying = false;
    void Awake()
    {
        player = GetComponent<VideoPlayer>();
        player.loopPointReached += EndReached;
    }


    void EndReached(VideoPlayer vp) {
        Debug.Log("end");
        rw.gameObject.SetActive(false);
    }

    /*
    void LatedUpdate()
    { 
        if (!player.isPlaying && isPlaying) {
            isPlaying = false;
            rw.enabled = false;
        }
        Debug.Log(player.isPlaying);
    }
    */

    public void Playing() {
        rw.enabled = true;
        rw.gameObject.SetActive(true);
        player.Play();
        isPlaying = true;
    }
}
