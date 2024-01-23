using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_audio_manager : MonoBehaviour {
    private static AudioSource source; //Audio source, in external scene to reduce strain on game

    private void Awake() {
        source = GetComponent<AudioSource>();
    }
    public static void playEffect(AudioClip clip) { //Play single effect
        source.PlayOneShot(clip);
    }
    public static void playEffect(AudioClip[] clips) { //Play multiple effects, go for walk SFX
        int rand = Random.Range(0, clips.Length);
        source.PlayOneShot(clips[rand]);
    }
}
