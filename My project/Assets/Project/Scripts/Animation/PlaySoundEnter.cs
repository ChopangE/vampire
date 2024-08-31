using Data;
using Manager;
using UnityEngine;

namespace SmallHedge.SoundManager
{
    public class PlaySoundEnter : StateMachineBehaviour
    {
        [SerializeField] private SFXEnum sound;
        [SerializeField, Range(0, 1)] private float volume = 1;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Global.SoundManager.PlaySFX(sound, volume);
        }
    }
}