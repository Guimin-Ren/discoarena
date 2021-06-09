using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AudioManager : NetworkBehaviour
{
    // Start is called before the first frame update
    public static AudioManager instance;
    public AudioSource BGM_Game;
    public AudioSource SoundEffect;
    public AudioClip mirrorStartEffect;
    public AudioClip mirrorEndEffect;
    public AudioClip mirrorLoopEffect;
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // call this function to play the BGM at all clients
    [ClientRpc]
    public void PlayBGM() {
        if (!BGM_Game.isPlaying) {
            BGM_Game.Play();
        }
        
    }
    
    //call this to play audio effect when mirror touch a laser
    [ClientRpc]
    public void MirrorStart() {
        if (SoundEffect.clip == mirrorStartEffect && SoundEffect.isPlaying) {
            return;
        }
        SoundEffect.Stop();
        SoundEffect.clip = mirrorStartEffect;
        SoundEffect.loop = false;
        SoundEffect.Play();
 
    }
  
    // call this when mirror escape from a laser
    [ClientRpc]
    public void MirrorEndPlay() {
        if (SoundEffect.clip == mirrorEndEffect && SoundEffect.isPlaying) {
            return;
        }
        MirrorLoopEnd();
        SoundEffect.clip = mirrorEndEffect;
        SoundEffect.Play();
    }
    //stop playing mirror audio effect
    [ClientRpc]
    public void MirrorLoopEnd() {
        SoundEffect.Stop();
        SoundEffect.loop = false;
    }
}
